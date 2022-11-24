using System;

namespace ISA_Marcin_Ryba_Lab03
{
	public static class StaticValues
	{
		public static Random Rand => _localRandom ??= new Random();
		[ThreadStatic] private static Random _localRandom;
		
		public static string Platform;
		
		public static TargetFunction TargetFunction = TargetFunction.Max;

		public static int L;
		
		public static double A;
		public static double B;
		public static double D;
		public static double Pk = 0.5;
		public static double Pm = 0.0005;
		
		public static double RandomXReal()
		{
			var accuracy = MathHelper.Accuracy(D);
			var trueXReal = Rand.NextDouble() * (B - A) + A;
			return Math.Round(trueXReal, accuracy);
		}
		
		public static double GetRandomDouble()
		{
			return Rand.NextDouble();
		}
	}
}