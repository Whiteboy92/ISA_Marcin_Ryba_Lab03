using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using static System.Double;

namespace ISA_Marcin_Ryba_Lab03
{
	public class DataRow
	{
		public string ChildXBin = null;
		public string MutatedChromosomeValue = null;
		
		public readonly long Index;
		
		public DataRow ParentsWith = null;
		public static readonly DataRow Empty = new DataRow(null, -1);
		
		public SortedSet<int> GenesValueAfterMutation = new SortedSet<int>();

		public readonly Values OriginalValues;
		public Values SelectedValue;
		
		public double GetRandomDouble;
		private double _randomizedParent = PositiveInfinity;
		public double GxValue;
		public double PxValue;
		public double QxValue;
		public double FinalXRealValue;
		public double FinalFxRealValue;
		
		public int? PcValue = null;

		public bool IsEliteTakingOver = false;
		public bool IsParent => _randomizedParent < StaticValues.Pk;

		public DataRow(Values originalValues, long index)
		{
			OriginalValues = originalValues;
			Index = index;
		}

		public void RandomizeSelection()
		{
			GetRandomDouble = StaticValues.Rand.NextDouble();
		}
		
		public void RandomizeParenting()
		{
			_randomizedParent = StaticValues.Rand.NextDouble();
		}

		public (string, string) N => ("N", Index.ToString());
		
		public (string, string) XReal => ("xReal", OriginalValues?.XReal.ToString(CultureInfo.CurrentCulture));
		
		public (string, string) Fx => ("F(x)", OriginalValues?.Fx.ToString("N20").TrimEnd('0'));
		
		public (string, string) Gx => ("G(x)", GxValue.ToString("N20").TrimEnd('0'));
		
		public (string, string) Px => ("P(x)", PxValue.ToString("N20").TrimEnd('0'));
		
		public (string, string) Qx => ("Q(x)", QxValue.ToString("N20").TrimEnd('0'));
		
		public (string, string) R1 => ("r", GetRandomDouble.ToString("N20").TrimEnd('0'));

		public (string, string) SelectXReal => ("select xReal", SelectedValue?.XReal.ToString(CultureInfo.CurrentCulture));
		private (string, string) SelectXBin => ("select xBin", SelectedValue?.XBin);

		public (string, string) FirstParentXBin => ("First Parent", IsParent ? SelectXBin.Item2 : "-");
		public (string, string) SecondParentXBin => ("Second Parent", ParentsWith != null ? ParentsWith.SelectXBin.Item2 : "-");
		public (string, string) Pc => ("PC", PcValue != null ? PcValue.ToString() : "-");
		
		public (string, string) Child => ("Child", ChildXBin ?? "-");

		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")] 
		public (string, string) AfterCrossing => ("Crossing", ChildXBin != null ? ChildXBin.Replace(" | ", "") : SelectXBin.Item2);

		public (string, string) MutatedGenes => ("Mutated Genes", GenesValueAfterMutation.Count > 0 ? GenesValueAfterMutation.Aggregate("", (s, i) => $"{s},{i+1}").Substring(1) : "-");
		
		public (string, string) MutatedChromosome => ("M xBin", IsEliteTakingOver ? "Elite Took Over" : MutatedChromosomeValue ?? "-");
		
		public (string, string) FinalXReal => ("M xReal", FinalXRealValue.ToString(CultureInfo.CurrentCulture));
		
		public (string, string) FinalFxReal => ("M F(x)", FinalFxRealValue.ToString("N25").TrimEnd('0'));
	}
}