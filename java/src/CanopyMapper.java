package solution;

import java.io.IOException;

import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Mapper;
import java.util.ArrayList;


public class CanopyMapper extends Mapper<LongWritable, Text, IntWritable, StockVector> {

	ArrayList<StockVector> CanpoyCenters = new ArrayList<StockVector>();
	IntWritable IntKey = new IntWritable(1);
	double T1 = Nasdaq.T1;
	double T2 = Nasdaq.T2;
  @Override
  public void map(LongWritable key, Text value, Context context)
      throws IOException, InterruptedException {
	  
	  	StockVector currStock = new StockVector(value);
	  	
	  	if(CanpoyCenters.size() == 0){
	  		
	  		StockVector addedVector = new StockVector(currStock);
	  		CanpoyCenters.add(addedVector);
	  	}
	  	else
	  	{
		  	boolean isCovered = false;
		  	
	  		for(StockVector currCenter : CanpoyCenters){
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
	  		
	  		if(!isCovered){
	  			
	  			StockVector addedVector = new StockVector(currStock);
		  		CanpoyCenters.add(addedVector);
	  		}
	  	}
  }
  
  @Override
  public void cleanup(Context context)
      throws IOException, InterruptedException {
	  
		for(StockVector currCenter : CanpoyCenters){
				context.write(IntKey, currCenter);
			}
		
		System.out.println("finish mapper cleanup");
  }
}