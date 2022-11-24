using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using ScottPlot.Plottable;
using static System.Double;

namespace ISA_Marcin_Ryba_Lab03
{
	public sealed partial class MainForm
	{
		private void FindBestParams()
		{
			if (!(FormatChecker.ParseDouble(_bestParamsAInput.Text, "A", out var a) &&
			      FormatChecker.ParseDouble(_bestParamsBInput.Text, "B", out var b) &&
			      FormatChecker.ParseDouble(_bestParamsDInput.SelectedKey, "D", out var d, "en-US") &&
			      FormatChecker.ParseLong(_bestParamsLoopCountInput.Text, "Loop Count", out var lc)
			    ))
			{
				return;
			}

			var nValues = _bestParamsNInput.Text.Split(';').Where(x => x.Trim().Length > 0).Select(x =>
			{
				FormatChecker.ParseLong(x, "N", out var n);
				return n;
			}).ToArray();


			var tValues = _bestParamsTInput.Text.Split(';').Where(x => x.Trim().Length > 0).Select(x =>
			{
				FormatChecker.ParseLong(x, "T", out var n);
				return n;
			}).ToArray();

			var pkValues = _bestParamsPkValue.Text.Split(';').Where(x => x.Trim().Length > 0).Select(x =>
			{
				FormatChecker.ParseDouble(x, "PK", out var pk);
				return pk;
			}).ToArray();


			var pmValues = _bestParamsPmValue.Text.Split(';').Where(x => x.Trim().Length > 0).Select(x =>
			{
				FormatChecker.ParseDouble(x, "PM", out var pm);
				return pm;
			}).ToArray();
			
			StaticValues.TargetFunction = _targetFunctionDropdown.SelectedKey switch
			{
				"MAX" => TargetFunction.Max,
				"MIN" => TargetFunction.Min,
				_ => throw new ArgumentOutOfRangeException()
			};

			var cumulatedDataRows = new List<BestParamsDataRow>();

			var elite = _bestParamsIsEliteCheckbox.Checked != null && _bestParamsIsEliteCheckbox.Checked.Value;
			StaticValues.D = d;
			StaticValues.A = a;
			StaticValues.B = b;
			var l = (int)Math.Floor(Math.Log((b - a) / d, 2) + 1.0);
			StaticValues.L = l;

			ClearBestParamsOutputTable();

			for (long n = 0; n < nValues.Length; n++)
			{
				for (long t = 0; t < tValues.Length; t++)
				{
					for (long pk = 0; pk < pkValues.Length; pk++)
					{
						StaticValues.Pk = pkValues[pk];
						for (long pm = 0; pm < pmValues.Length; pm++)
						{
							StaticValues.Pm = pmValues[pm];
							var fXValues = new List<double>();
							for (long i = 0; i < lc; i++)
							{
								var data = new DataRow[nValues[n]];
								InitializeData(data, nValues[n]);

								for (var j = 0; j < tValues[t]; j++)
								{
									CalculateGx(data);
									CalculatePx(data);
									CalculateQx(data);
									Selection(data);
									Parenting(data);
									PairParents(data);
									RandomizePc(data);
									MakeBabies(data);
									Mutate(data, elite);
									Finalize(data);

									if (j + 1 < tValues[t])
									{
										MoveToNextGeneration(data);
									}
								}

								fXValues.Add(data.Average(x => x.FinalFxRealValue));
							}

							cumulatedDataRows.Add(new BestParamsDataRow()
							{
								NValue = nValues[n],
								Value = tValues[t],
								PkValue = pkValues[pk],
								PmValue = pmValues[pm],
								AvgFxValue = fXValues.Average()
							});
						}
					}
				}
			}

			if (StaticValues.TargetFunction == TargetFunction.Max)
			{
				cumulatedDataRows = cumulatedDataRows.OrderByDescending(x => x.AvgFxValue)
					.ThenBy(x => x.NValue * x.Value).ToList();
			}
			else
			{
				cumulatedDataRows = cumulatedDataRows.OrderBy(x => x.AvgFxValue).ThenBy(x => x.NValue * x.Value)
					.ToList();
			}
			AddBestParamsDataToTable(cumulatedDataRows);
		}

		private void ExecuteGeneration()
		{
			if (!(
				    FormatChecker.ParseDouble(_aInput.Text, "A", out var a) &&
				    FormatChecker.ParseDouble(_bInput.Text, "B", out var b) &&
				    FormatChecker.ParseLong(_nInput.Text, "N", out var n) &&
				    FormatChecker.ParseDouble(_dInput.SelectedKey, "D", out var d, "en-US") &&
				    FormatChecker.ParseDouble(_pkInput.Text, "PK", out var pk) &&
				    FormatChecker.ParseDouble(_pmInput.Text, "PM", out var pm) &&
				    FormatChecker.ParseLong(_tInput.Text, "Loop Count", out var lc)
			    )
			   )
			{
				return;
			}

			if (n < 0)
			{
				MessageBox.Show("N can't be less than 0", MessageBoxType.Error);
				return;
			}

			if (lc < 1)
			{
				MessageBox.Show("Loop Count should be 1 or greater", MessageBoxType.Error);
				return;
			}

			var l = (int)Math.Floor(Math.Log((b - a) / d, 2) + 1.0);
			var elite = _isEliteCheckbox.Checked != null && _isEliteCheckbox.Checked.Value;

			StaticValues.A = a;
			StaticValues.B = b;
			StaticValues.D = d;
			StaticValues.L = l;
			StaticValues.Pk = pk;
			StaticValues.Pm = pm;

			StaticValues.TargetFunction = _targetFunctionDropdown.SelectedKey switch
			{
				"MAX" => TargetFunction.Max,
				"MIN" => TargetFunction.Min,
				_ => throw new ArgumentOutOfRangeException()
			};
			
			ClearCumulatedDataOutputTable();
			
			var data = new DataRow[n];
			InitializeData(data, n);
			var maximalFxValues = new List<double> {data.Max(dataRow => dataRow.OriginalValues.Fx)};
			var averageFxValues = new List<double> {data.Average(dataRow => dataRow.OriginalValues.Fx)};
			var minimalFxValues = new List<double> {data.Min(dataRow => dataRow.OriginalValues.Fx)};


			var maximalGxValues = new SignalPlot
			{
				Color = Color.Red,
				SampleRate = 1,
				MinRenderIndex = 0
			};
			
			var averageGxValues = new SignalPlot
			{
				Color = Color.ForestGreen,
				SampleRate = 1,
				MinRenderIndex = 0
			};
			
			var minimalGxValues = new SignalPlot
			{
				Color = Color.MediumBlue,
				SampleRate = 1,
				MinRenderIndex = 0
			};
			
			_plotView.Reset();
			_plotView.Plot.Add(maximalGxValues);
			_plotView.Plot.Add(averageGxValues);
			_plotView.Plot.Add(minimalGxValues);


			for (var i = 0; i < lc; i++)
			{
				CalculateGx(data);
				CalculatePx(data);
				CalculateQx(data);
				Selection(data);
				Parenting(data);
				PairParents(data);
				RandomizePc(data);
				MakeBabies(data);
				Mutate(data, elite);
				Finalize(data);

				minimalFxValues.Add(data.Min(x => x.FinalFxRealValue));
				maximalFxValues.Add(data.Max(x => x.FinalFxRealValue));
				averageFxValues.Add(data.Average(x => x.FinalFxRealValue));

				if (i + 1 < lc)
				{
					MoveToNextGeneration(data);
				}
			}

			minimalGxValues.Ys = minimalFxValues.ToArray();
			minimalGxValues.MaxRenderIndex = minimalGxValues.PointCount - 1;
			maximalGxValues.Ys = maximalFxValues.ToArray();
			maximalGxValues.MaxRenderIndex = maximalGxValues.PointCount - 1;
			averageGxValues.Ys = averageFxValues.ToArray();
			averageGxValues.MaxRenderIndex = averageGxValues.PointCount - 1;

			_plotView.Plot.AxisAuto(0.05f, 0.1f);
			_plotView.Refresh();

			AddCumulatedDataToTable(new[]{new CumulatedDataRow(){XBinValue = ""}});
			Task.Run(() => CumulateData(data));
		}

		internal class DataGroup : IComparer<DataGroup>, IComparable<DataGroup>
		{
			public readonly double XReal;
			public readonly double Percent;
			private readonly double _fx;

			public DataGroup(double xReal, double percent, double fx)
			{
				XReal = xReal;
				Percent = percent;
				_fx = fx;
			}

			public int CompareTo(DataGroup other)
			{
				if (ReferenceEquals(this, other)) return 0;
				return ReferenceEquals(null, other) ? 1 : _fx.CompareTo(other._fx);
			}

			public int Compare(DataGroup x, DataGroup y)
			{
				if (ReferenceEquals(x, y)) return 0;
				if (ReferenceEquals(null, y)) return 1;
				if (ReferenceEquals(null, x)) return -1;
				return x._fx.CompareTo(y._fx);
			}
		}

		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		private void Finalize(DataRow[] data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			Parallel.ForEach(data, row =>
			{
				row.FinalXRealValue = MathHelper.XBinToXReal(row.MutatedChromosomeValue);
				row.FinalFxRealValue = MathHelper.Fx(row.FinalXRealValue);
			});
		}
		
		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		[SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH")]
		private void MoveToNextGeneration(DataRow[] data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			for (var i = 0; i < data.Length; i++)
			{
				data[i] = new DataRow(new Values(data[i].FinalXRealValue), data[i].Index);
			}
		}
		
		private void CumulateData(IReadOnlyCollection<DataRow> data)
		{
			var getXRealAmount = new Dictionary<double, long>();
			foreach (var dataRow in data)
			{
				if (getXRealAmount.ContainsKey(dataRow.FinalXRealValue))
				{
						getXRealAmount[dataRow.FinalXRealValue]++;
				}
				else
				{
						getXRealAmount.Add(dataRow.FinalXRealValue, 1);
				}
			}

			var countDataAsDouble = data.Count * 0.01;
			var cumulatedData = new SortedSet<DataGroup>();

			foreach (var xRealCount in getXRealAmount)
			{ 
				cumulatedData.Add(new DataGroup(xRealCount.Key, xRealCount.Value / countDataAsDouble, MathHelper.Fx(xRealCount.Key)));
			}

			var dataGroups = (StaticValues.TargetFunction == TargetFunction.Max) ? cumulatedData : cumulatedData.Reverse();
			var groups = new CumulatedDataRow[cumulatedData.Count];

			long i = 0;
			foreach (var dataGroup in dataGroups)
			{ 
				groups[i] = new CumulatedDataRow(i + 1, dataGroup.XReal, dataGroup.Percent);
				i++;
			}
			Application.Instance.InvokeAsync(() => AddCumulatedDataToTable(groups));
		}

		private void InitializeData(DataRow[] data, long n)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			for (var i = 0; i < n; i++)
			{
				var values = new Values();
				data[i] = new DataRow(values, i + 1);
			}
		}

		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		[SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH")]
		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		[SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH")]
		private void Mutate(DataRow[] data, bool isElite)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			long eliteXInt = 0;
			double highestFxValue = 0;
			if (isElite)
			{
				var highestGxValue = MinValue;
				foreach (var row in data)
				{
					if (!(row.GxValue > highestGxValue)) continue;
					eliteXInt = row.OriginalValues.XInt;
					highestGxValue = row.GxValue;
					highestFxValue = row.OriginalValues.Fx;
				}
			}

			var eliteTookOver = !isElite;

			Parallel.ForEach(data, dataRow =>
			{
				dataRow.GenesValueAfterMutation = new SortedSet<int>();
				var chromosome = dataRow.AfterCrossing.Item2.ToCharArray();
				for (var j = 0; j < StaticValues.L; j++)
				{
					if (!(StaticValues.Rand.NextDouble() < StaticValues.Pm)) continue;
					dataRow.GenesValueAfterMutation.Add(j);
					chromosome[j] = chromosome[j] == '0' ? '1' : '0';
				}

				var xBin = new string(chromosome);
				dataRow.MutatedChromosomeValue = xBin;
				if (eliteTookOver) return;
				var xInt = MathHelper.XBinToXInt(xBin);
				if (xInt == eliteXInt)
				{
					eliteTookOver = true;
					return;
				}

				var xReal = MathHelper.XIntToXReal(xInt);
				var fxValue = MathHelper.Fx(xReal);
				if (StaticValues.TargetFunction == TargetFunction.Max)
				{
					if (fxValue > highestFxValue)
					{
						eliteTookOver = true;
					}
				}
				else
				{
					if (fxValue < highestFxValue)
					{
						eliteTookOver = true;
					}
				}
			});


			if (!isElite || eliteTookOver) return;
			var index = StaticValues.Rand.Next(0, data.Length - 1);
			data[index].IsEliteTakingOver = true;
			data[index].MutatedChromosomeValue = MathHelper.XIntToXBin(eliteXInt);
		}

		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		private void MakeBabies(DataRow[] data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			Parallel.ForEach(data, dataRow =>
			{
				if (dataRow.ParentsWith == null) return;
				if (dataRow.PcValue != null)
					dataRow.ChildXBin =
						$"{dataRow.FirstParentXBin.Item2.Substring(0, dataRow.PcValue.Value)} | {dataRow.SecondParentXBin.Item2.Substring(dataRow.PcValue.Value)}";
			});
		}

		private void RandomizePc(DataRow[] data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			foreach (var dataRow in data)
			{
				if (dataRow.ParentsWith == null || dataRow.PcValue != null) continue;
				var pc = 1 + (int) Math.Round(StaticValues.GetRandomDouble() * (StaticValues.L - 2));
				dataRow.PcValue = pc;
				dataRow.ParentsWith.PcValue = pc;
			}
		}
		
		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		private void CalculatePx(DataRow[] data)
		{
			var sum = data.Sum(dataRow => dataRow.GxValue);
			Parallel.ForEach(data, dataRow => { dataRow.PxValue = dataRow.GxValue / sum; });
		}

		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		private static void CalculateGx(DataRow[] data)
		{
			switch (StaticValues.TargetFunction)
			{
				case TargetFunction.Max:
					var min = data.Min(x => x.OriginalValues.Fx);
					Parallel.ForEach(data,
						dataRow => { dataRow.GxValue = dataRow.OriginalValues.Fx - min + StaticValues.D; });
					break;
				
				case TargetFunction.Min:
					var max = data.Max(x => x.OriginalValues.Fx);
					Parallel.ForEach(data,
						dataRow => { dataRow.GxValue = -(dataRow.OriginalValues.Fx - max) + StaticValues.D; });
					break;
				
				default:
					throw new NotImplementedException();
			}
		}

		private void PairParents(IReadOnlyList<DataRow> data)
		{
			for (var i = 0; i < data.Count; i++)
			{
				var row = data[i];
				if (!row.IsParent || row.ParentsWith != null) continue;
				DataRow pair = null;
				if (i + 1 < data.Count)
				{
					for (var j = i + 1; j < data.Count; j++)
					{
						if (!data[j].IsParent) continue;
						pair = data[j];
						break;
					}
				}

				if (pair == null) continue;
				row.ParentsWith = pair;
				pair.ParentsWith = row;
			}
		}

		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		private void Parenting(DataRow[] data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			Parallel.ForEach(data, row => { row.RandomizeParenting(); });
		}

		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
		private void Selection(DataRow[] data)
		{
			Parallel.ForEach(data, dataRow =>
			{
				try
				{
					dataRow.RandomizeSelection();
					var selectedIndex = MathHelper.FindQWithBinarySearch(data, dataRow.GetRandomDouble);
					if (selectedIndex == -1)
					{ 
						MessageBox.Show($"Iter Limit Reached for {dataRow.GetRandomDouble}"); 
						selectedIndex = 0;
					}

					dataRow.SelectedValue = data[selectedIndex].OriginalValues;
				}
				catch (Exception e)
				{
					MessageBox.Show(e.ToString());
				}
			});
		}

		private void CalculateQx(DataRow[] data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			var sum = 0.0;
			foreach (var dataRow in data)
			{
				sum += dataRow.PxValue;
				dataRow.QxValue = sum;
			}
		}
	}
}