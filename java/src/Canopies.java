package solution;

import java.io.DataInput;
import java.io.DataOutput;
import java.io.IOException;
import java.util.ArrayList;

import org.apache.hadoop.io.Text;

public class Canopies {
	private ArrayList<StockVector> canopies;
	private int totalStocks;

	public Canopies() {
		super();
		canopies = new ArrayList<StockVector>();
		totalStocks = 0;
	}

	public StockVector get(int i)
	{
		return canopies.get(i);
	}

	public int size()
	{
		return canopies.size();
	}

	public int canopySize()
	{
		if (canopies.isEmpty())
			return 0;

		return canopies.get(0).size();
	}

	public int getTotalStocks()
	{
		return totalStocks;
	}

	public void addCanopy(StockVector canopy)
	{
		StockVector canopyToAdd = new StockVector(canopy);
		canopies.add(canopyToAdd);
		totalStocks += canopyToAdd.GetSum() + 1;
	}
}