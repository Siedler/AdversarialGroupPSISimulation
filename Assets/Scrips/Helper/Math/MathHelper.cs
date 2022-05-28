namespace Scrips.Helper.Math {
	public class MathHelper {

		public static int GaussianSum(int n) {
			return (n * n + 1) / 2;
		}
		
		public static double RunningAverage(double avg, double newDataPoint, double alpha = .1) {
			return (alpha * newDataPoint) + (1.0 - alpha) * avg;
		}
	}
}