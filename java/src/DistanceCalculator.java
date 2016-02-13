package solution;

public class DistanceCalculator {
	
	public static final double compareDistance(Vector v1, Vector v2){
		double sum = 0;
		int size = v1.size();
		
		for(int i = 0; i < size; i++){
			try
			{
				// Uclidian 
				//sum += Math.pow(Math.abs(v2.getValue(i) - v1.getValue(i)), 2);
				// Manhattan
				sum += Math.abs(v2.getValue(i) - v1.getValue(i));
			}
			catch(Exception ex)
			{
				ex.equals(ex);
			
			}
		}
	
		// Uclidian
		//return Math.sqrt(sum);
		// Manhattan
		return sum;
	}
}
