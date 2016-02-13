package solution;

import java.io.IOException;
import java.net.URI;
import java.util.ArrayList;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.filecache.DistributedCache;
import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.SequenceFile;

public class DistributedCacheUtils {

    public static void writeCanopyKMeansKey(Configuration conf, CanopyKMeansKey key) throws IOException
    {
        FileSystem fs = FileSystem.get(conf);

        IntWritable IntKey = new IntWritable(1);
        Path keyFileName = new Path(key.getkMeansCentroid()
        							   .getId()
        							   .toString());

        // create file
        @SuppressWarnings("deprecation")
		final SequenceFile.Writer writer = SequenceFile.createWriter
								(fs, conf, keyFileName, CanopyKMeansKey.class, IntWritable.class);

        // write key to file
        writer.append(key, IntKey);
        
        // close writer and file system
        writer.close();
        //fs.close();
        
        // add file to configuration cache
        DistributedCache.addCacheFile(keyFileName.toUri(), conf);
    }
    
    public static void writeCanopyKMeansKeys(Configuration conf, ArrayList<CanopyKMeansKey> keys) throws IOException
    {
		for (CanopyKMeansKey key : keys)
		{
			// write key to distributed cache
			writeCanopyKMeansKey(conf, key);
		}
    }
    
    public static CanopyKMeansKey readCanopyKMeansKey(Configuration conf, Path keyFilePath) throws IOException
    {
    	CanopyKMeansKey key = new CanopyKMeansKey();
    	
		try
		{
			FileSystem fs = FileSystem.get(conf);
			@SuppressWarnings("deprecation")
			SequenceFile.Reader reader = new SequenceFile.Reader(fs, keyFilePath, conf);
			
			IntWritable value = new IntWritable();
			// read canopy kmeans key
			reader.next(key, value);
			// close reader and file system
			reader.close();
			//fs.close();
		}
		catch (IOException ioe)
		{
			System.err.println("Caught exception while parsing the cached file '" + keyFilePath.toString());
		}
		
		return key;
    }
    
    public static ArrayList<CanopyKMeansKey> readCanopyKMeansKeys(Configuration conf) throws IOException
    {
    	ArrayList<CanopyKMeansKey> keys = new ArrayList<CanopyKMeansKey>();
    	
		// read cached files
		URI[] cachedFiles = DistributedCache.getCacheFiles(conf);
		for (URI cacheFile : cachedFiles)
		{
			Path cacheFilePath = new Path(cacheFile.getPath());
			// parse the canopy an kmeans mapping file
			CanopyKMeansKey key = new CanopyKMeansKey(readCanopyKMeansKey(conf, cacheFilePath));
			// add to keys
			keys.add(key);
		}
		
		return keys;
    }
    
    public static void writeCanopyCenters(Configuration conf, ArrayList<StockVector> canopyCenters) throws IOException
    {
    	FileSystem fs = FileSystem.get(conf);

        IntWritable IntKey = new IntWritable(1);
        Path canopyFileName = new Path(Nasdaq.CANOPY_SEQ_FILE_PATH);
        
        System.out.println("before seq file");
        // create file
        @SuppressWarnings("deprecation")
		final SequenceFile.Writer writer = SequenceFile.createWriter
								(fs, conf, canopyFileName, StockVector.class, IntWritable.class);

        System.out.println("after seq file");
        System.out.println("canopies" + canopyCenters.size());
        for(StockVector canopyCenter : canopyCenters)
        {
            // write canopy to file
            writer.append(canopyCenter, IntKey);
            System.out.println("sum " + canopyCenter.GetSum());
        }
        System.out.println("canopies end" + canopyCenters.size());
        // close writer and file system
        writer.close();
        //fs.close();
    }


    public static Canopies readCanopyCenters(Configuration conf) throws IOException
    {
    	Canopies canopies = new Canopies();
    	
    	FileSystem fs = FileSystem.get(conf);

    	Path canopyFileName = new Path(Nasdaq.CANOPY_SEQ_FILE_PATH);
		// init canopies
		@SuppressWarnings("deprecation")
		SequenceFile.Reader reader = new SequenceFile.Reader(fs, canopyFileName ,	conf);

		StockVector canopy = new StockVector();
		IntWritable value = new IntWritable();

		while (reader.next(canopy, value))
		{
			// parse the canopy center
			StockVector canopyToAdd = new StockVector(canopy);
			// add to canopy centers
			canopies.addCanopy(canopyToAdd);
		}

		reader.close();
		//fs.close();
		
		return canopies;
    }
}
