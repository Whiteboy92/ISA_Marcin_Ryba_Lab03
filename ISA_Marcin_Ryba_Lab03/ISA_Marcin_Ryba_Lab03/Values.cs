namespace ISA_Marcin_Ryba_Lab03
{
	public class Values
	{
		public readonly double XReal;
		public readonly long XInt;
		public readonly string XBin;
		public readonly double Fx;

		public Values()
		{
			XReal = StaticValues.RandomXReal();
			XInt = MathHelper.XRealToXInt(XReal);
			XBin = MathHelper.XIntToXBin(XInt);
			Fx = MathHelper.Fx(XReal);
		}
		
		public Values(double xReal)
		{
			XReal = xReal;
			XInt = MathHelper.XRealToXInt(xReal);
			XBin = MathHelper.XIntToXBin(XInt);
			Fx = MathHelper.Fx(xReal);
		}
	}
}