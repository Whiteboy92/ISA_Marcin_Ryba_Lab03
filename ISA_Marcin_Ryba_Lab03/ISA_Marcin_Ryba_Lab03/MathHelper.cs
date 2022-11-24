using System;

namespace ISA_Marcin_Ryba_Lab03
{
	public static class MathHelper
	{
		public static int Accuracy(double d)
		{
			return d switch
			{
				1.0 => 0,
				0.1 => 1,
				0.01 => 2,
				0.001 => 3,
				_ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
			};
		}

		public static long XBinToXInt(string xBin)
		{
			return Convert.ToInt64(xBin, 2);
		}

		public static double XIntToXReal(long xInt)
		{
			var trueXReal = ((StaticValues.B - StaticValues.A) * xInt) / (Math.Pow(2.0, StaticValues.L) - 1.0) + StaticValues.A;
			return Math.Round(trueXReal, Accuracy(StaticValues.D));
		}
		
		public static double XBinToXReal(string xBin)
		{
			return XIntToXReal(XBinToXInt(xBin));
		}

		public static double Fx(double xReal)
		{
			return (xReal % 1.0) * (Math.Cos(20.0 * Math.PI * xReal) - Math.Sin(xReal));
		}

		public static long XRealToXInt(double xReal)
		{
			return (long)Math.Round((1.0 / (StaticValues.B - StaticValues.A)) * (xReal - StaticValues.A) * ((Math.Pow(2.0, StaticValues.L)) - 1.0));
		}

		public static string XIntToXBin(long xInt)
		{
			return Convert.ToString(xInt, 2).PadLeft(StaticValues.L, '0');
		}
		
		public static long FindQWithBinarySearch(DataRow[] data, double select) {
			long minimalNumber = 0;
			long maximalNumber = data.Length - 1;

			if (maximalNumber == 0)
			{
				return maximalNumber;
			}
			
			if (data[minimalNumber].QxValue >= select)
			{
				return 0;
			}
			
			if (data[maximalNumber].QxValue <= select)
			{
				return maximalNumber;
			}

			if (data[maximalNumber].QxValue >= select && data[maximalNumber - 1].QxValue <= select)
			{
				return maximalNumber;
			}

			long i = 0;
			while (minimalNumber <= maximalNumber) {
				var midValue = (minimalNumber + maximalNumber) / 2;
				if (minimalNumber == maximalNumber)
				{
					return midValue;
				}
				if (data[midValue].QxValue >= select)
				{
					if (data[midValue - 1].QxValue <= select)
					{
						return midValue;
					}

					maximalNumber = midValue;
				}
				else
				{
					minimalNumber = midValue;
				}

				i++;
				
				if (i > data.Length)
				{
					return -1;
				}
			}
			return 0;
		}
	}
}