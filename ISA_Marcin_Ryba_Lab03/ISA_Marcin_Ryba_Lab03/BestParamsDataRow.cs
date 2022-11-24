namespace ISA_Marcin_Ryba_Lab03
{
	public class BestParamsDataRow
	{
		public static readonly BestParamsDataRow Empty = new BestParamsDataRow();
		
		public long NValue;
		public long Value;
		
		public double PkValue;
		public double PmValue;
		public double AvgFxValue;
		
		public (string, string) N => ("N", NValue.ToString("D"));
		
		public (string, string) T => ("T", Value.ToString("D"));
		
		public (string, string) Pk => ("PK", PkValue.ToString("0." + new string('#', 55)));
		
		public (string, string) Pm => ("PM", PmValue.ToString("0." + new string('#', 55)));
		
		public (string, string) Cost => ("Price", (NValue * Value).ToString("D"));
		
		public (string, string) AvgFx => ("Avg F(x)", AvgFxValue.ToString("0." + new string('#', 55)));
	}
}