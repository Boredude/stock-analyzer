package solution;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Reducer;

import solution.Vector;

// calculate a new clustercenter for these vertices
public class KMeansReducer extends
		Reducer<CanopyKMeansKey, StockVector, Text, Text> {

	public static enum Counter {
		CONVERGED,
		REDUCED
	}
		
	@Override
	protected void reduce(CanopyKMeansKey key, Iterable<StockVector> values,
			Context context) throws IOException, InterruptedException {
		
		// save stock vectors to cache
		List<StockVector> stocks = new ArrayList<StockVector>();
		for(StockVector stock : values){
			stocks.add(new StockVector(stock));
		}
		
		// debug printing
        System.out.println("Reducing: " + key.getkMeansCentroid().getId() 
        					+ " - " + stocks.size() + " stocks associated");
        // increase the reduced centroids counter to indicate how many centroids
        // have any stocks associated with them (are not empty)
        context.getCounter(Counter.REDUCED).increment(1);
		
		// init new centroid
		int size = key.getkMeansCentroid().size();
		Vector  newCentroid = new Vector(size, key.getkMeansCentroid().getId().toString());

		// foreach vetcor feature
		for (int i=0; i < size; i++)
		{
			// iterate through the stocks
			for(StockVector stock : stocks){

				// sum the values for current feature
				double val = newCentroid.getValue(i);
				val += stock.getValue(i);
				newCentroid.setValue(i, val);

			}

			// calculate feature average
			double sum = newCentroid.getValue(i);
			newCentroid.setValue(i, (sum / stocks.size()));
		}

		// create the new key
		CanopyKMeansKey newKey = new CanopyKMeansKey(key.getCanopy(), newCentroid);

		// write new key to distributed cache
		DistributedCacheUtils.writeCanopyKMeansKey(context.getConfiguration(), newKey);

		// if center did not update increment converged count
		if (key.compareTo(newKey) == 0)
			context.getCounter(Counter.CONVERGED).increment(1);

		Configuration conf = context.getConfiguration();
		// check if we need to write the output
		if (conf.getBoolean("shouldWriteOutput", false))
		{
			// write each stock name with it's center's id
			for(StockVector stock : stocks){
				// write stock id with it's associated K center
				context.write(stock.getId(), 
							  newKey.getkMeansCentroid().getId());
			}
		}
	}
}
