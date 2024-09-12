using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2Column : UserControl {
	#region private
	private const double MinColumnWidth = 25;
	private const double SplitterWidth = 1;

	private int _columnIndex = 0;

	private GridLength _length = new(1, GridUnitType.Star);
	private IBinding? _binding;
	private Grid? _grid = null;

	private ColumnDefinition _columnSplitter = new(SplitterWidth, GridUnitType.Pixel);
	private GridSplitter _splitter;
	#endregion

	#region public
	public static readonly DirectProperty<TreeDataView2Column, IDataTemplate?> HeaderTemplateProperty =
		AvaloniaProperty.RegisterDirect<TreeDataView2Column, IDataTemplate?>(nameof(HeaderTemplate), x => x.HeaderTemplate, (x, template) => x.HeaderTemplate = template);

	public static readonly DirectProperty<TreeDataView2Column, GridLength> ColumnWidthProperty =
		AvaloniaProperty.RegisterDirect<TreeDataView2Column, GridLength>(nameof(ColumnWidth), x => x.ColumnWidth, (x, width) => x.ColumnWidth = width);
	
	public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
		AvaloniaProperty.Register<TreeDataView2Column, IDataTemplate?>(nameof(ItemTemplate));

	public static readonly StyledProperty<IDataTemplate?> CellContentTemplateProperty =
		AvaloniaProperty.Register<TreeDataView2Column, IDataTemplate?>(nameof(CellContentTemplate));

	public static readonly StyledProperty<string?> HeaderProperty =
		AvaloniaProperty.Register<TreeDataView2Column, string?>(nameof(Header));

	public static readonly StyledProperty<ColumnDefinition> ColumnHeaderProperty =
		AvaloniaProperty.Register<TreeDataView2Column, ColumnDefinition>(nameof(ColumnHeader));

	public IDataTemplate? HeaderTemplate {
		get => GetValue(ContentTemplateProperty);
		set => SetValue(ContentTemplateProperty, value);
	}

	public IDataTemplate? ItemTemplate {
		get => GetValue(ItemTemplateProperty);
		set => SetValue(ItemTemplateProperty, value);
	}

	public IDataTemplate? CellContentTemplate {
		get => GetValue(CellContentTemplateProperty);
		set => SetValue(CellContentTemplateProperty, value);
	}

	public string? Header {
		get => GetValue(HeaderProperty);
		set => SetValue(HeaderProperty, value);
	}

	public GridLength ColumnWidth {
		get => ColumnHeader.Width;
		set => ColumnHeader.Width = value; // Raise...
	}

	public ColumnDefinition ColumnHeader {
		get => GetValue(ColumnHeaderProperty);
		set => SetValue(ColumnHeaderProperty, value);
	}

	public IBinding? Binding {
		get => _binding;
		set => _binding = value;
	}
	#endregion

	public TreeDataView2Column() {
		InitializeComponent();

		ColumnHeader = new(1, GridUnitType.Star) { MinWidth = MinColumnWidth };

		_splitter = GenerateSplitter();
	}

	private GridSplitter GenerateSplitter() => new GridSplitter() {
		Background = Brushes.Red,
		HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
	};

	public void Append(Grid grid) { 
		if (_grid != null) {
			Remove();
		}

		_columnIndex = grid.ColumnDefinitions.Count;

		grid.ColumnDefinitions.Add(ColumnHeader);

		grid.Children.Add(this);
		grid.Children.Add(_splitter);

		Grid.SetColumn(this, _columnIndex);
		Grid.SetColumn(_splitter, _columnIndex);

		_grid = grid;
	}

	public void Remove() {
		if (_grid == null) return;

		_grid.Children.Remove(this);
		_grid.Children.Remove(_splitter);
		
		_grid.ColumnDefinitions.Remove(ColumnHeader);

		_grid = null;
	}

	public void BuildCell(TreeDataView2 view, TreeDataView2Row row, object? param, INameScope scope, int i) {
		Binding bWidth = new Binding() {
			//Path = $"Columns[{i}].ColumnHeader.Width",
			//Source = view,
			Path = $"{nameof(ColumnHeader)}.{nameof(Width)}",
			Source = this,
			Mode = BindingMode.TwoWay,
		};

		row.PART_GridCells.ColumnDefinitions.Add(new ColumnDefinition() { [!ColumnDefinition.WidthProperty] = bWidth, });

		if (ItemTemplate == null) {
			return;
		}

		Control? control = ItemTemplate.Build(param);
		if (control is not TreeDataView2Cell cell) {
			return;
		}

		//cell[!TreeDataView2Cell.IsVisibleProperty] = new Binding() { Path = nameof(IsVisible), Source = this };
		cell[!TreeDataView2Cell.DataContextProperty] = new Binding(nameof(DataContext));
		cell[!TreeDataView2Cell.CellContentTemplateProperty] = new Binding(nameof(CellContentTemplate), BindingMode.TwoWay) { Source = this };

		ApplyBindings(cell);

		Grid.SetColumn(cell, _columnIndex);
		row.PART_GridCells.Children.Add(cell);
	}

	public virtual void ApplyBindings(Control cell) { }

	//protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) {
	//	if (e.Property == TreeDataView2Column.IsVisibleProperty) {
	//		if (e.NewValue is bool bVisible && e.OldValue != e.NewValue) {
	//			if (bVisible) {
	//				ColumnHeader.Width = _length;
	//				ColumnHeader.MinWidth = MinColumnWidth;
	//				_columnSplitter.Width = new(SplitterWidth, GridUnitType.Pixel);
	//			}
	//			else {
	//				_length = ColumnWidth;
	//				ColumnHeader.Width = new(0, GridUnitType.Pixel);
	//				ColumnHeader.MinWidth = 0;
	//				_columnSplitter.Width = new(0, GridUnitType.Pixel);
	//			}
	//		}
	//	}
	//
	//	base.OnPropertyChanged(e);
	//}
}