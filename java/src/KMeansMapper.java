package solution;

import java.io.IOException;
import java.util.ArrayList;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Mapper;

import solution.Vector;

// first iteration, k-random centers, in every follow-up iteration we have new calculated centers
public class KMeansMapper extends
		Mapper<LongWritable, Text, CanopyKMeansKey, StockVector> {

	private ArrayList<CanopyKMeansKey> canopyKMeansKeys = new ArrayList<CanopyKMeansKey>();
	private StockVector relatedCanopy;
	private ArrayList<Vector> relatedKmeansCentroids;

	@Override
	protected void setup(Context context) throws IOException,
			InterruptedException {
		super.setup(context);
		Configuration conf = context.getConfiguration();
		// read from distributed cache all the keys
		canopyKMeansKeys = DistributedCacheUtils.readCanopyKMeansKeys(conf);
	}

	@Override
	protected void map(LongWritable key, Text value, Context context)
			throws IOException, InterruptedException {

		// read line to stock vector
		StockVector currStock = new StockVector(value);

		// iterate through the canopies and figure out which one
		// the stock vector belongs to
		relatedKmeansCentroids = getCanopyDataByStockVector(currStock);
		// for the given canopy, find the K inside that canopy which
		// the stock vector is closest to
		Vector closestKmeansCentroid = getCentroidByStockVector(currStock, relatedKmeansCentroids);
		// write the key with the given stock vector
		CanopyKMeansKey outputKey = new CanopyKMeansKey(relatedCanopy, closestKmeansCentroid);
		// write data to reducer
		context.write(outputKey, currStock);
	}

	private Vector getCentroidByStockVector(StockVector vector, ArrayList<Vector> KMeansCenters)
	{
		double minimumDistance = Double.MAX_VALUE;
		Vector closestCentroid = null;
		// iterate through the canopies
		for(Vector centroid: KMeansCenters) {
			// check if the distance from the given stock vector
			// to the current canopy's associated K means centroids.
			// check if it is lower than the currently miniimum distance
			double currentDistance = DistanceCalculator.compareDistance(vector, centroid);
			if (currentDistance < minimumDistance)
			{
				// update the related canopy data and the minimum distance
				minimumDistance = currentDistance;
				closestCentroid = centroid;
			}
		}

		return closestCentroid;
	}

	private ArrayList<Vector> getCanopyDataByStockVector(StockVector vector)
	{
		double minimumDistance = Double.MAX_VALUE;
		// iterate through the canopies
		for(CanopyKMeansKey key : canopyKMeansKeys) {
			// check if the distance from the given stock vector
			// to the current canopy is lower than the currently
			// minimum distance
			double currentDistance = DistanceCalculator.compareDistance(vector, key.getCanopy());
			if (currentDistance < minimumDistance)
			{
				// update the related canopy data and the minimum distance
				minimumDistance = currentDistance;
				relatedCanopy = key.getCanopy();
			}
		}

		// return all the K centroids related with this stock's canopy
		ArrayList<Vector> relatedCentroids = new ArrayList<Vector>();

		for(CanopyKMeansKey key : canopyKMeansKeys) {
			// if this k means centroid is related to the related canopy we found
			if (key.getCanopy().compareTo(relatedCanopy) == 0)
				// add it to list
				relatedCentroids.add(key.getkMeansCentroid());
		}

		return relatedCentroids;
	}
}
