using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using ScottPlot.Eto;

namespace ISA_Marcin_Ryba_Lab03
{
	public partial class MainForm : Form
	{
		private TextBox _aInput;
		private TextBox _bInput;
		private TextBox _nInput;
		private TextBox _pkInput;
		private TextBox _pmInput;
		private TextBox _tInput;
		
		private CheckBox _isEliteCheckbox;
		
		private Button _startButton;
		
		private DropDown _dInput;
		
		private GridView _cumulatedDataOutputTable;
		
		//--------------------------------------------------------------------------------------
		
		private TextBox _bestParamsAInput;
		private TextBox _bestParamsBInput;
		private TextBox _bestParamsLoopCountInput;
		private TextBox _bestParamsTInput;
		private TextBox _bestParamsNInput;
		private TextBox _bestParamsPkValue;
		private TextBox _bestParamsPmValue;
		
		private DropDown _bestParamsDInput;

		private Button _bestParamsStartButton;
		
		private GridView _bestParamsOutputTable;

		private CheckBox _bestParamsIsEliteCheckbox;

		private readonly TabControl _tabsControl;
		
		private readonly TabPage _plotViewTabPage;
		private readonly TabPage _cumulatedDataTabPage;
		
		private readonly PlotView _plotView;
		private readonly StackLayout _initializeBasicControls;

		private DropDown _targetFunctionDropdown;
		private DropDown _analysisTargetFunctionDropdown;
		
		private const int PanelWidth = 15;

		public MainForm()
		{
			Title = "ISA_Marcin_Ryba_lab03";
			Width = 1350;
			Height = 600;
			MinimumSize = new Size(1350, 600);
			Resizable = true;
			_initializeBasicControls = InitializeBasicControls();
			var bestParamsControls = InitializeBestParamsControls();
			
			CreateCumulatedDataOutputTable();
			CreateBestParamsOutputTable();
			_plotView = new PlotView();
			_plotView.Width = 800;
			_plotView.Height = 600;
			
			_cumulatedDataTabPage = new TabPage()
			{
				Content = new StackLayout()
				{
					Orientation = Orientation.Vertical,
					Padding = 10,
					Items =
					{
						_initializeBasicControls,
						_cumulatedDataOutputTable
					}
				},
				Text = "Values Page"
			};

			_plotViewTabPage = new TabPage()
			{
				Content = new StackLayout()
				{
					Orientation = Orientation.Vertical,
					Padding = 10,
					Items =
					{
						_plotView
					}
				},
				Text = "Plot Page"
			};
			
			var analysisResultsPage = new TabPage()
			{
				Content = new StackLayout()
				{
					Orientation = Orientation.Vertical,
					Padding = 10,
					Items =
					{
						bestParamsControls,
						_bestParamsOutputTable
					}
				},
				Text = "Find Best Params"
			};
			

			_tabsControl = new TabControl()
			{
				Pages =
				{
					_cumulatedDataTabPage, _plotViewTabPage,  analysisResultsPage
				}
			};

			_tabsControl.SelectedIndexChanged += TabsControlIndexDisplay;

			Content = new StackLayout
			{
				Orientation = Orientation.Vertical,
				Padding = 10,
				Items =
				{
					_tabsControl
				}
			};

			SizeChanged += (sender, args) =>
			{
				if (_cumulatedDataOutputTable != null)
				{
					_cumulatedDataOutputTable.Width = Width - 70;
					_cumulatedDataOutputTable.Height = Height - 155;
				}

				if (_bestParamsOutputTable != null)
				{
					_bestParamsOutputTable.Width = Width - 70;
					_bestParamsOutputTable.Height = Height - 155;
				}
				if (_plotView != null)
				{
					_plotView.Width = Width - 100;
					_plotView.Height = Height - 125;
				}

				if (_tabsControl == null) return;
				_tabsControl.Width = Width - 40;
				_tabsControl.Height = Height - 60;
			};
		}

		private StackLayout InitializeBestParamsControls()
		{
			_bestParamsAInput = new TextBox()
			{
				Text = "-4",
				Width = 45
			};
			_bestParamsBInput = new TextBox()
			{
				Text = "12",
				Width = 45
			};
			_bestParamsDInput = new DropDown()
			{
				Items = { "1", "0.1", "0.01", "0.001" },
				SelectedIndex = 3
			};
			
			_bestParamsPkValue = new TextBox()
			{
				Text = $"{0.50:0.00};{0.60:0.00};{0.70:0.00};{0.80:0.00};{0.90:0.00}",
				Width = 100
			};
			
			_bestParamsPmValue = new TextBox()
			{
				Text = $"{0.0001:0.0000};{0.0005:0.0000};{0.001:0.000};{0.005:0.000};{0.01:0.00};{0.05:0.00}",
				Width = 100
			};
			
			_bestParamsNInput = new TextBox()
			{
				Text = $"{30};{40};{50};{60};{70};{80}",
				Width = 100
			};

			_bestParamsTInput = new TextBox()
			{
				Text = $"{50};{60};{70};{80};{90};{100};{110};{120};{130};{140};{150}",
				Width = 100
			};

			_bestParamsLoopCountInput = new TextBox()
			{
				Text = "100",
				Width = 45
			};
			
			_bestParamsStartButton = new Button()
			{
				Text = "Start",
				Command = new Command((sender, e) => FindBestParams())
			};
			
			_analysisTargetFunctionDropdown = new DropDown()
			{
				Items = { "MAX", "MIN" },
				SelectedIndex = 0
			};
			
			_bestParamsIsEliteCheckbox = new CheckBox()
			{
				Checked = true,
				ThreeState = false
			};
			
			return new StackLayout()
			{
				Orientation = Orientation.Horizontal,
				AlignLabels = true,
				VerticalContentAlignment = VerticalAlignment.Center,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				Padding = 10,
				Items =
				{
					LabelOverride("A: "),
					_bestParamsAInput,
					PanelOverride(),
					LabelOverride("B: "),
					_bestParamsBInput,
					PanelOverride(),
					LabelOverride("D: "),
					_bestParamsDInput,
					PanelOverride(),
					LabelOverride("isElite: "),
					_bestParamsIsEliteCheckbox,
					PanelOverride(),
					LabelOverride("Target: "),
					_analysisTargetFunctionDropdown,
					PanelOverride(),
					LabelOverride("PK: "),
					_bestParamsPkValue,
					PanelOverride(),
					LabelOverride("PM: "),
					_bestParamsPmValue,
					PanelOverride(),
					LabelOverride("N: "),
					_bestParamsNInput,
					PanelOverride(),
					LabelOverride("T: "),
					_bestParamsTInput,
					PanelOverride(),
					LabelOverride("Loop Count: "),
					_bestParamsLoopCountInput,
					PanelOverride(),
					LabelOverride("Loop Count: "),
					_bestParamsStartButton
				}
			};
		}
		
		private void TabsControlIndexDisplay(object sender, EventArgs e)
		{
			switch (_tabsControl.SelectedIndex)
			{
				case 0:
					_cumulatedDataTabPage.Content = new StackLayout()
					{
						Orientation = Orientation.Vertical,
						Padding = 10,
						Items =
						{
							_initializeBasicControls,
							_cumulatedDataOutputTable
						}
					};
					break;
				case 1:
					_plotViewTabPage.Content = new StackLayout()
					{
						Orientation = Orientation.Vertical,
						Padding = 10,
						Items =
						{
							_plotView
						}
					};
					break;
			}
		}
		
		private void ClearCumulatedDataOutputTable()
		{
			_cumulatedDataOutputTable.DataStore = Array.Empty<CumulatedDataRow>();
		}

		private void ClearBestParamsOutputTable()
		{
			_bestParamsOutputTable.DataStore = Array.Empty<BestParamsDataRow>();
		}

		private void AddCumulatedDataToTable(IEnumerable<CumulatedDataRow> dataGroups)
		{
			_cumulatedDataOutputTable.DataStore = dataGroups;
		}

		private void AddBestParamsDataToTable(IEnumerable<BestParamsDataRow> bestParamsData)
		{
			_bestParamsOutputTable.DataStore = bestParamsData;
		}

		private void CreateCumulatedDataOutputTable()
		{
			_cumulatedDataOutputTable = new GridView()
			{
				DataStore = Array.Empty<CumulatedDataRow>(),
				Width = Width - 42
			};

			foreach (var property in typeof(CumulatedDataRow).GetProperties())
			{
				if (property.PropertyType != typeof((string, string))) continue;

				_cumulatedDataOutputTable.Columns.Add(new GridColumn()
				{
					HeaderText = (((string title, string))property.GetValue(CumulatedDataRow.Empty)).title,
					DataCell = new TextBoxCell()
					{
						Binding = Binding.Property<CumulatedDataRow, string>(x =>
							(((string, string value))property.GetValue(x)).value)
					}
				});
			}
		}

		private void CreateBestParamsOutputTable()
		{
			_bestParamsOutputTable = new GridView()
			{
				DataStore = Array.Empty<BestParamsDataRow>(),
				Width = Width - 42
			};

			foreach (var property in typeof(BestParamsDataRow).GetProperties())
			{
				if (property.PropertyType != typeof((string, string))) continue;

				_bestParamsOutputTable.Columns.Add(new GridColumn()
				{
					HeaderText = (((string title, string))property.GetValue(BestParamsDataRow.Empty)).title,
					DataCell = new TextBoxCell()
					{
						Binding = Binding.Property<BestParamsDataRow, string>(x =>
							(((string, string value))property.GetValue(x)).value)
					}
				});
			}
		}

		private StackLayout InitializeBasicControls()
		{
			_aInput = new TextBox()
			{
				Text = "-4"
			};
			_bInput = new TextBox()
			{
				Text = "12"
			};
			_dInput = new DropDown()
			{
				Items = { "1", "0.1", "0.01", "0.001" },
				SelectedIndex = 3
			};
			_nInput = new TextBox()
			{
				Text = "80"
			};
			
			_pkInput = new TextBox()
			{
				Text = "0.9"
			};
			
			_pmInput = new TextBox()
			{
				Text = "0.0001"
			};
			
			_targetFunctionDropdown = new DropDown()
			{
				Items = { "MAX", "MIN" },
				SelectedIndex = 0
			};

			_tInput = new TextBox()
			{
				Text = "110"
			};

			_isEliteCheckbox = new CheckBox()
			{
				ThreeState = false,
				Checked = true
			};
			
			_startButton = new Button()
			{
				Text = "Start",
				Command = new Command((sender, args) => ExecuteGeneration())
			};

			var controls = new StackLayout()
			{
				Orientation = Orientation.Horizontal,
				AlignLabels = true,
				VerticalContentAlignment = VerticalAlignment.Center,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				Padding = 10,
				Items =
				{
					LabelOverride("A: "),
					_aInput,
					PanelOverride(),
					LabelOverride("B: "),
					_bInput,
					PanelOverride(),
					LabelOverride("D: "),
					_dInput,
					PanelOverride(),
					LabelOverride("N: "),
					_nInput,
					PanelOverride(),
					LabelOverride("PK: "),
					_pkInput,
					PanelOverride(),
					LabelOverride("PM: "),
					_pmInput,
					PanelOverride(),
					LabelOverride("Target: "),
					_targetFunctionDropdown,
					PanelOverride(),
					LabelOverride("T: "),
					_tInput,
					PanelOverride(),
					LabelOverride("isElite: "),
					_isEliteCheckbox,
					PanelOverride(),
					_startButton
				}
			};
			return controls;
		}

		private static Label LabelOverride(string text, string tooltip = null)
		{
			if (tooltip == null)
			{
				return new Label()
				{
					Text = text
				};
			}

			return new Label()
			{
				Text = text,
				ToolTip = tooltip
			};
		}

		private static Panel PanelOverride()
		{
			return new Panel()
			{
				Width = PanelWidth,
			};
		}
	}
}