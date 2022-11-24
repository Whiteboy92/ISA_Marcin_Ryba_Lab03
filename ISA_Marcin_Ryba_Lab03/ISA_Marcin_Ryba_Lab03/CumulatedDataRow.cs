using System.Globalization;

namespace ISA_Marcin_Ryba_Lab03
{
	public class CumulatedDataRow
	{
		public static readonly CumulatedDataRow Empty = new CumulatedDataRow();

		public CumulatedDataRow()
		{
		}

		public CumulatedDataRow(long index, double xRealValue, double percentValue)
		{
			_index = index;
			_xRealValue = xRealValue;
			XBinValue = MathHelper.XIntToXBin(MathHelper.XRealToXInt(xRealValue));
			_fxValue = MathHelper.Fx(xRealValue);
			_percentValue = percentValue;
		}


		private readonly long _index;
		private readonly double _xRealValue;
		public string XBinValue = "";
		private readonly double _fxValue;
		private readonly double _percentValue;
		
		public (string, string) N => ("N", _index.ToString());
		public (string, string) XReal => ("xReal", _xRealValue.ToString(CultureInfo.CurrentCulture));
		public (string, string) XBin => ("xBin", XBinValue);
		public (string, string) Fx => ("F(x)", _fxValue.ToString(CultureInfo.CurrentCulture));
		public (string, string) Percent => ("%", _percentValue.ToString(CultureInfo.CurrentCulture));
	}
}