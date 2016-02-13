package solution;

import java.io.File;
import java.io.IOException;
import java.net.URI;
import java.util.ArrayList;

import org.apache.commons.io.FileUtils;
import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Job;
import org.apache.hadoop.mapreduce.lib.input.FileInputFormat;
import org.apache.hadoop.mapreduce.lib.output.FileOutputFormat;

public class Nasdaq {

    // CONSTANTS
    //
    public final static String CANOPY_SEQ_FILE_PATH = "Nasdaq/canopyCen.seq";
    public final static double T1 = 4250;
    public final static double T2 = 0;

	public static void main(String[] args) throws Exception {

		if (args.length != 3) {

			System.exit(-1);
		}

        // run canopy clustering
        Job canopyJob = createCanopyJob(args[0], args[1]);

        // delete output if exists
        deleteOutputFolder(args[1]);

		boolean success = canopyJob.waitForCompletion(true);
		
		if (success)
        {
			System.out.println("canopy success");
            // read canopies
            Canopies canopies = DistributedCacheUtils.readCanopyCenters(canopyJob.getConfiguration());

			// delete output if exists
            deleteOutputFolder(args[1]);

            // get the K
			int k = Integer.parseInt(args[2]);
            // generate relative k centroid for each canopy
			ArrayList<CanopyKMeansKey> keys = createCanopyKMeansKeys(canopies, k);
			System.out.println("read success");
			// Counter to indicate the number of convereged centroid in a k means run
			long convergedCounter = -1;
			// Counter to indicate the number of reduced (non empty) centroids in a k means run
			long reducedCounter = 0;
			// Counter of k means runs
            int kRun = 0;
			while (convergedCounter < reducedCounter) {
				System.out.println("run kmeans num " + kRun);
				// increase k
                kRun++;
                // create job
                Job kMeansJob = createKMeansJob(args[0],  args[1]);
                // write keys to job's cache
                DistributedCacheUtils.writeCanopyKMeansKeys(kMeansJob.getConfiguration(), keys);
				// delete output if exists
                deleteOutputFolder(args[1]);

				// run job
				success = kMeansJob.waitForCompletion(true);
                // re-read counters
				convergedCounter = kMeansJob.getCounters()
											.findCounter(KMeansReducer.Counter.CONVERGED)
											.getValue();
				reducedCounter = kMeansJob.getCounters()
										  .findCounter(KMeansReducer.Counter.REDUCED)
										  .getValue();

                // debug printing
                System.out.println("Kmeans run: " + kRun + ", centroids convereged so far: " + convergedCounter + 
                				   " of " + reducedCounter);

                // re-read and keys for next run
                keys = DistributedCacheUtils.readCanopyKMeansKeys(kMeansJob.getConfiguration());
			}
			
			// the last time after converged
			Job kMeansJob = createKMeansJob(args[0], args[1]);
            Configuration conf = kMeansJob.getConfiguration();

			// delete output if exists
            deleteOutputFolder(args[1]);

            // set should write output
            conf.setBoolean("shouldWriteOutput", true);
            
            // write converged keys to job's cache
            DistributedCacheUtils.writeCanopyKMeansKeys(conf, keys);

            // run for last time
			success = kMeansJob.waitForCompletion(true);
			
            // debug printing
            System.out.println("Kmeans run finished");
			
			System.exit(success ? 0 : 1);
		}
	}

    private static void deleteOutputFolder(String path) throws IOException {

        Configuration conf = new Configuration();
        conf.set("fs.hdfs.impl",org.apache.hadoop.hdfs.DistributedFileSystem.class.getName());
        conf.set("fs.file.impl",org.apache.hadoop.fs.LocalFileSystem.class.getName());
        FileSystem  hdfs = FileSystem.get(URI.create("hdfs://localhost:8020/"), conf);
        hdfs.delete(new Path(path), true);
    }

    private static Job createKMeansJob(String input,
									   String output) throws IOException
	{
		try
		{
			// delete output directory if exists
			//File outputDir = new File(output);
			//FileUtils.deleteDirectory(outputDir);
		}
        catch (Exception e)
		{
			e.printStackTrace();
		}

        Configuration conf = new Configuration();
		Job kMeansJob = new Job(conf);

		kMeansJob.setJobName("Nasdaq Stocks - Kmeans");
		kMeansJob.setJarByClass(Nasdaq.class);

		kMeansJob.setMapperClass(KMeansMapper.class);
		kMeansJob.setReducerClass(KMeansReducer.class);

		FileInputFormat.setInputPaths(kMeansJob, new Path(input));
		FileOutputFormat.setOutputPath(kMeansJob, new Path(output));

		kMeansJob.setOutputKeyClass(Text.class);
		kMeansJob.setOutputValueClass(Text.class);
		kMeansJob.setMapOutputKeyClass(CanopyKMeansKey.class);
		kMeansJob.setMapOutputValueClass(StockVector.class);

		return kMeansJob;
	}

	private static Job createCanopyJob(String input,
									   String output) throws IOException
	{
		try
		{
			//File outputDir = new File(output);
			//FileUtils.deleteDirectory(outputDir);
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}

		Configuration conf = new Configuration();
		Job canopyJob = new Job(conf);
		
		canopyJob.setJobName("Nasdaq Stocks - Canopy");
		canopyJob.setJarByClass(Nasdaq.class);

		canopyJob.setMapperClass(CanopyMapper.class);
		canopyJob.setReducerClass(CanopyReducer.class);
		
		FileInputFormat.setInputPaths(canopyJob, new Path(input));
		FileOutputFormat.setOutputPath(canopyJob, new Path(output));

		canopyJob.setOutputKeyClass(IntWritable.class);
		canopyJob.setOutputValueClass(StockVector.class);
		canopyJob.setOutputKeyClass(IntWritable.class);
		canopyJob.setOutputValueClass(StockVector.class);

		return canopyJob;
	}

    private static ArrayList<CanopyKMeansKey> createCanopyKMeansKeys(Canopies canopies, int K)
    {
        // calculate vector size
        int vecSize = canopies.canopySize();
        int remainK = K;
        ArrayList<CanopyKMeansKey> canopyKMeansKeys = new ArrayList<CanopyKMeansKey>();
        
        ArrayList<Integer> kList = CreateListOfKNum(K, canopies);
        // run all canopies
        for (int i = 0; i < canopies.size(); i++) {
        	// calc k amount
        	int knum = kList.get(i);
            
            System.out.println("Canopy " + i + " has " + knum + " K's");

            // calc random values
            double maxDis = T1 / vecSize;
            double minDis = -(T1 / vecSize);

            // for over all k
            for (int j = 0; j < knum; j++) {
                double range = (maxDis - minDis) + 1; // 2*T1+(1?)

                Vector currK = new Vector(vecSize, Integer.toString(i) + "_" + Integer.toString(j));

                // for each feature
                for (int f = 0; f < vecSize; f++) {
                    double randDis = Math.random() * range + minDis;
                    currK.setValue(f, randDis + canopies.get(i).getValue(f));
                }
                CanopyKMeansKey currKey = new CanopyKMeansKey(
                        canopies.get(i), currK);
                // add current k
                canopyKMeansKeys.add(currKey);
            }
        }

        return canopyKMeansKeys;
    }
    
    private static ArrayList<Integer> CreateListOfKNum(int k, Canopies canopies) {
		int canopiesNum = canopies.size();
		int kRemain = k;
		int nSum = 0;
		boolean bIsFit = false;
		ArrayList<Integer> result = new ArrayList<Integer>();
		for (int i = 0; i < canopiesNum; i++) {
			int knum = (int) Math.ceil(((double) (((double) (canopies.get(i)
					.GetSum() + 1) / canopies.getTotalStocks()) * k)));
			kRemain -= knum;
			if (knum == 1) {
				knum++;
				kRemain--;
			}
			while (!bIsFit) {
				// Check f we have left more then we need
				if ((kRemain / (canopiesNum - (i + 1))) < (canopiesNum - (i + 1))) {
					knum--;
					kRemain++;
				} else {
					bIsFit = true;
					break;
				}
			}
			nSum += knum;
			result.add(knum);
		}
		// If the round made overflow
		if(nSum > k)
		{
			for (int j =0; j<result.size() && (nSum != k); j++) {
				if(result.get(j) > 2)
				{
					result.set(j, (result.get(j) - 1));
					nSum--;
				}
			}
		}
		return result;
	}
}
