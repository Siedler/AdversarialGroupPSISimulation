using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

namespace Scrips.Helper.Math {
	public class MathHelper {

		public static int GaussianSum(int n) {
			return (n * n + 1) / 2;
		}
		
		public static double RunningAverage(double avg, double newDataPoint, double alpha = .1) {
			return (alpha * newDataPoint) + (1.0 - alpha) * avg;
		}

		public static double NextGaussian() {
			double v1, v2, s;
			
			do {
				v1 = 2.0 * Random.Range(0.0f, 1.0f) - 1.0;
				v2 = 2.0 * Random.Range(0.0f, 1.0f) - 1.0;
				s = v1 * v1 + v2 * v2;
			} while (s >= 1.0f || s == 0f);
			
			s = System.Math.Sqrt((-2.0 * System.Math.Log(s)) / s);

			return v1 * s;
		}

		public static double NextGaussian(double mean, double sigma) {
			return sigma * NextGaussian() + mean;
		}

		public static double NextGaussian(double mean, double sigma, double min, double max) {
			double gaussian = NextGaussian(mean, sigma);
			
			if (gaussian < min) gaussian = min;
			if (gaussian > max) gaussian = max;

			return gaussian;
		}

		public static double NextGaussianFromInterval(double minValue, double maxValue) {
			double mean = (minValue + maxValue) / 2;
			double sigma = (maxValue - mean) / 3;

			return NextGaussian(mean, sigma, minValue, maxValue);
		}
	}
}