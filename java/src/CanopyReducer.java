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
import org.apache.hadoop.mapreduce.Reducer;

public class CanopyReducer extends Reducer<IntWritable, StockVector, IntWritable, StockVector> {
	
	ArrayList<StockVector> canpoyCenters = new ArrayList<StockVector>();
	double T1 = Nasdaq.T1;
	double T2 = Nasdaq.T2;
	
  @Override
	public void reduce(IntWritable key, Iterable<StockVector> values, Context context)
			throws IOException, InterruptedException {
	  System.out.println("key " + key);
	  for (StockVector currStock : values) {
		  
	  	if(canpoyCenters.size() == 0){
	  		StockVector addedVector = new StockVector(currStock);
	  		canpoyCenters.add(addedVector);
	  	}
	  	else
	  	{
		  	boolean isCovered = false;
		  	
		  	for(StockVector currCenter : canpoyCenters){
	  			
	  			double Distance = DistanceCalculator.compareDistance(currCenter, currStock);
	  			if(Distance <= T1){
	 	  				isCovered = true;
	  				
	  				// the current vector is between the two T's
	  				if(Distance > T2)
	  				{
	  					currCenter.setSumCount(currStock.GetSum() + 1);
	  				}
	 	  				break;
	 	  		}
  			}
	  		if(!isCovered)
	  		{
	  			StockVector addedVector = new StockVector(currStock);
	  			canpoyCenters.add(addedVector);
	  		}
	  	}
	  }
	}
  
    @Override
    public void cleanup(Context context)
        throws IOException, InterruptedException {
    	System.out.println("cleanup canopy reduce");
		// write canopy centers to distributed cache
		//DistributedCacheUtils.writeCanopyCenters(context.getConfiguration(), canpoyCenters);
    	FileSystem fs = FileSystem.get(context.getConfiguration());

        IntWritable IntKey = new IntWritable(1);
        Path canopyFileName = new Path(Nasdaq.CANOPY_SEQ_FILE_PATH);
        
        System.out.println("before seq file");
        // create file
        @SuppressWarnings("deprecation")
		final SequenceFile.Writer writer = SequenceFile.createWriter
								(fs, context.getConfiguration(), canopyFileName, StockVector.class, IntWritable.class);

        System.out.println("after seq file");
        System.out.println("canopies" + canpoyCenters.size());
        for(StockVector canopyCenter : canpoyCenters)
        {
            // write canopy to file
            writer.append(canopyCenter, IntKey);
            System.out.println("sum " + canopyCenter.GetSum());
        }
        System.out.println("canopies end" + canpoyCenters.size());
        // close writer and file system
        writer.close();
        //fs.close();
    }
}