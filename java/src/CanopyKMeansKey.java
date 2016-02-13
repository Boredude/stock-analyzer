package solution;

import java.io.DataInput;
import java.io.DataOutput;
import java.io.IOException;

import org.apache.hadoop.io.Text;
import org.apache.hadoop.io.WritableComparable;

public class CanopyKMeansKey implements WritableComparable<CanopyKMeansKey> {

	 private StockVector canopy;
	    private Vector kMeansCentroid;

	    public CanopyKMeansKey()
	    {
	        super();
	        this.canopy = new StockVector();
	        this.kMeansCentroid = new Vector();
	    }

	    public CanopyKMeansKey(StockVector canopy, Vector kMeansCentroid) {
	        super();
	        this.canopy = new StockVector(canopy);
	        this.kMeansCentroid = new Vector(kMeansCentroid);
	    }
	    
	    public CanopyKMeansKey(CanopyKMeansKey key) {
	        super();
	        this.canopy = new StockVector(key.getCanopy());
	        this.kMeansCentroid = new Vector(key.getkMeansCentroid());
	    }

	    public void write(DataOutput out) throws IOException {
	        this.canopy.write(out);
	        this.kMeansCentroid.write(out);
	    }

	    public void readFields(DataInput in) throws IOException {
	        this.canopy.readFields(in);
	        this.kMeansCentroid.readFields(in);
	    }

	    public int compareTo(CanopyKMeansKey o) {

	        // compare the canopies
	        int result = this.canopy.compareTo(o.canopy);
	        if (result == 0)
	            // compare the kmeans centroid
	            return this.kMeansCentroid.compareTo(o.kMeansCentroid);

	        // otherwise return the result of the canopy comparer
	        return result;
	    }

	    @Override
	    public int hashCode(){

			return canopy.hashCode() ^ kMeansCentroid.hashCode();
		}

	    @Override
	    public String toString() {
	        // TODO: change A the centroid id
	        return "Canopy id: " + canopy.getId() + 
	        	   ", K Centroid id:" + kMeansCentroid.getId() + 
	        	   ", [" + kMeansCentroid.getValue(0) +
	        	   ", "+ kMeansCentroid.getValue(1) +
	        	   ", " +kMeansCentroid.getValue(2) + "]";
	    }

	    public Vector getkMeansCentroid()
	    {
	        return kMeansCentroid;
	    }

	    public StockVector getCanopy()
	    {
	        return canopy;
	    }
}