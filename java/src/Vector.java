package solution;

import java.io.DataInput;
import java.io.DataOutput;
import java.io.IOException;
import java.util.Arrays;

import org.apache.hadoop.io.Text;
import org.apache.hadoop.io.WritableComparable;

public class Vector implements WritableComparable<Vector> {

	protected Text id;
	protected double[] values;

	public Vector() {
		super();
		id = new Text();
	}
	
	public Vector(int size){
		super();
        this.id = new Text();
		values = new double[size];
	}
	
	public Vector(int size, String id){
		super();
        this.id = new Text(id);
		values = new double[size];
	}

	public Vector(Vector v) {
		super();
		int l = v.values.length;
		this.values = new double[l];
		this.id = new Text(v.id);
		System.arraycopy(v.values, 0, this.values, 0, l);
	}


	public void write(DataOutput out) throws IOException {
		id.write(out);
		out.writeInt(values.length);
		for (int i = 0; i < values.length; i++)
			out.writeDouble(values[i]);
	}

	public void readFields(DataInput in) throws IOException {
		id.readFields(in);
		int size = in.readInt();
		values = new double[size];
		for (int i = 0; i < size; i++)
            values[i] = in.readDouble();
	}

	public int compareTo(Vector o) {
		
		for (int i = 0; i < values.length; i++) {
			double c = values[i] - o.values[i];
			if (c!= 0.00d)
			{
				return (int)c;
			}		
		}
		return 0;
	}

    public int size()
    {
        return values.length;
    }


	public void setId(Text id){
		this.id = id;
	}

	public Text getId(){
		return id;
	}

    public double getValue(int position) {
        return values[position];
    }

    public void setValue(int position, double value) {
        values[position] = value;
    }

	public double[] getValues() {
		return values;
	}

	public void setValues(double[] values) {
		this.values = values;
	}

	@Override
	public String toString() {
		return "Vector id: " + this.id + " [values=" + Arrays.toString(values) + "]";
	}

}
