package solution;

import java.io.DataInput;
import java.io.DataOutput;
import java.io.IOException;

import org.apache.hadoop.io.Text;

public class StockVector extends Vector {

	private int sumCount;

	public StockVector() {
		super();
		sumCount = 0;
	}

	public StockVector(StockVector vector){
		super((Vector)vector);
		sumCount = vector.GetSum();
	}
	
	public StockVector(Text line) {

        super();

		String[] strVector = line.toString()
                                 .split(",");

        int size = strVector.length - 1;
        sumCount = 0;

        // set vector's id
		this.id = new Text(strVector[0]);

        // set vector's length (amount of values)
        this.values = new double[size];

        // parse values
		for (int i = 0; i < size; i++){
			this.values[i] = Double.parseDouble(strVector[i + 1]);
		}
	}

	@Override
	public void write(DataOutput out) throws IOException {
		super.write(out);
		out.writeInt(sumCount);
	}

	@Override
	public void readFields(DataInput in) throws IOException {
        super.readFields(in);
		sumCount = in.readInt();
	}

	public int GetSum(){
		return sumCount;
	}

	public void setSumCount(int toAdd)
	{
		this.sumCount += toAdd;
	}

}