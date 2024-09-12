using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2 : UserControl {
	#region private
	private TreeDataView2ExpanderColumn? _expanderColumn;
	private TreeDataView2ItemsPresenter? _itemsPresenter;

	private event Func<object?, bool>? _filterItems;
	#endregion

	#region public
	#region event
	public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
		RoutedEvent.Register<TreeDataView2, SelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

	public static readonly RoutedEvent<ItemClickEventArgs> ClickRowEvent =
		RoutedEvent.Register<TreeDataView2, ItemClickEventArgs>(nameof(ClickRow), RoutingStrategies.Bubble);

	public static readonly RoutedEvent<ItemClickEventArgs> DoubleClickRowEvent =
		RoutedEvent.Register<TreeDataView2, ItemClickEventArgs>(nameof(DoubleClickRow), RoutingStrategies.Bubble);

	public event EventHandler<SelectionChangedEventArgs> SelectionChanged {
		add => AddHandler(SelectionChangedEvent, value);
		remove => RemoveHandler(SelectionChangedEvent, value);
	}

	public event EventHandler<ItemClickEventArgs> ClickRow {
		add => AddHandler(ClickRowEvent, value);
		remove => RemoveHandler(ClickRowEvent, value);
	}

	public event EventHandler<ItemClickEventArgs> DoubleClickRow {
		add => AddHandler(DoubleClickRowEvent, value);
		remove => RemoveHandler(DoubleClickRowEvent, value);
	}
	#endregion

	public static readonly StyledProperty<SelectionMode> SelectionModeProperty =
		AvaloniaProperty.Register<TreeDataView2, SelectionMode>(nameof(SelectionMode), SelectionMode.Single);

	public static readonly StyledProperty<TreeDataView2Item?> SelectedItemProperty =
		AvaloniaProperty.Register<TreeDataView2, TreeDataView2Item?>(nameof(SelectedItem));

	public static readonly StyledProperty<List<TreeDataView2Item>> SelectedItemsProperty =
		AvaloniaProperty.Register<TreeDataView2, List<TreeDataView2Item>>(nameof(SelectedItems), new());

	public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
		AvaloniaProperty.Register<TreeDataView2, IEnumerable?>(nameof(ItemsSource));

	//public static readonly DirectProperty<TreeDataView2, IEnumerable?> ItemsSourceProperty =
	//	AvaloniaProperty.RegisterDirect<TreeDataView2, IEnumerable?>(nameof(ItemsSource), x => x.ItemsSource, (x, value) => x.ItemsSource = value);

	public static readonly StyledProperty<ObservableCollection<TreeDataView2Column>> ColumnsProperty =
		AvaloniaProperty.Register<TreeDataView2, ObservableCollection<TreeDataView2Column>>(nameof(Columns));

	public event Func<object?, bool> FilterItems {
		add {
			_filterItems += value;

			UpdateFilter();
		}
		remove {
			_filterItems -= value;
		}
	}

	public IEnumerable? ItemsSource {
		get => GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	public ObservableCollection<TreeDataView2Column> Columns {
		get => GetValue(ColumnsProperty);
		private set => SetValue(ColumnsProperty, value);
		//get;
	}

	public SelectionMode SelectionMode {
		get => GetValue(SelectionModeProperty);
		set => SetValue(SelectionModeProperty, value);
	}

	public TreeDataView2Item? SelectedItem {
		get => GetValue(SelectedItemProperty);
		set => SetValue(SelectedItemProperty, value);
	}

	public List<TreeDataView2Item> SelectedItems {
		get => GetValue(SelectedItemsProperty);
		set => SetValue(SelectedItemsProperty, value);
	}

	private TreeDataView2ItemRoot? _itemRoot;
	#endregion

	public TreeDataView2() {
        InitializeComponent();

		Columns = new();
		Columns.CollectionChanged += Columns_CollectionChanged;
		// Columns = new();
	}

	private TreeDataView2Row? BuildRow(object? @object, INameScope scope) {
		TreeDataView2Row item = new() { };

		for (int i = 0; i < Columns.Count; i++) {
			if (Columns[i].IsVisible)
				Columns[i].BuildCell(this, item, @object, scope, i);
		}

		return item;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
		if (change.Property == ItemsSourceProperty) {
			Dispatcher.UIThread.Post(ResetSource, DispatcherPriority.Background);
		}
		
		base.OnPropertyChanged(change);
	}

	//protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
	//	if (change.Property == ColumnsProperty) {
	//		if (change.OldValue is ObservableCollection<TreeDataView2Column> observableOld) {
	//			observableOld.CollectionChanged -= Columns_CollectionChanged;

	//			TreeDataView2Column[] columns = PART_GridHeaders.Children.OfType<TreeDataView2Column>().ToArray();
	//			foreach (var it in columns) {
	//				it.RemoveFromGrid();
	//			}
	//		}

	//		if (change.NewValue is ObservableCollection<TreeDataView2Column> observableNew) {
	//			observableNew.CollectionChanged += Columns_CollectionChanged;

	//			foreach (var it in observableNew) {
	//				it.AppendToGrid(PART_GridHeaders);
	//			}
	//		}
	//	}

	//	base.OnPropertyChanged(change);
	//}

	private void Columns_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
		switch (e.Action) {
			case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
				if (e.NewItems == null)
					break;
				foreach (var it in e.NewItems.OfType<TreeDataView2Column>()) {
					//TODO: remove it
					if (it is TreeDataView2ExpanderColumn expander) {
						_expanderColumn = expander;
					}

					it.PropertyChanged += ViewColumn_PropertyChanged;

					//if (it.IsVisible) {
					//	it.AppendToGrid(PART_GridHeaders);
					//}
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				if (e.OldItems == null)
					break;
				foreach (var it in e.OldItems.OfType<TreeDataView2Column>()) {
					it.PropertyChanged -= ViewColumn_PropertyChanged;
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
				TreeDataView2Column[] columns = PART_GridHeaders.Children.OfType<TreeDataView2Column>().ToArray();
				foreach (var it in columns) {
					it.PropertyChanged -= ViewColumn_PropertyChanged;
				}
				break;
			// TODO: остальные действия над коллекцией
			default:
				break;
		}

		Reset();
	}

	private void ViewColumn_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) {
		if (e.Property == TreeDataView2Column.IsVisibleProperty) {
			Reset();
		}
	}

	private void ResetColumns() {
		foreach (TreeDataView2Column column in Columns)
			column.Remove();

		for (int i = 0; i < Columns.Count; i++) {
			if (Columns[i].IsVisible)
				Columns[i].Append(PART_GridHeaders);
		}
	}

	public void Reset() {
		ResetColumns();

		_itemsPresenter = new();
		_itemsPresenter.PART_ItemsRepeater.ItemTemplate = new FuncDataTemplate(@object => @object != null, BuildRow);

		PART_BorderRows.Child = _itemsPresenter;

		if (_itemRoot != null) {
			_itemsPresenter!.ItemRoot = _itemRoot;
		}
	}

	private void ResetSource() {
		IBinding? innerItemBinding = null;
		IBinding? isExpandedBinding = null;

		if (_expanderColumn != null) {
			innerItemBinding = _expanderColumn.InnerItemsSource;
			isExpandedBinding = _expanderColumn.IsExpanded;
		}

		_itemRoot = new(ItemsSource, innerItemBinding, isExpandedBinding);
		_itemRoot.OnViewFilter += OnViewFilter;
		_itemsPresenter!.ItemRoot = _itemRoot;
	}

	//private void TreeDataView2_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e) {
	//	if (SelectedItem == null) return;

	//	if (e.KeyModifiers == Avalonia.Input.KeyModifiers.None) {
	//		int index = ItemRoot.ItemsView.IndexOf(SelectedItem);
	//		if (index < 0) return;

	//		switch (e.Key) {
	//			case Avalonia.Input.Key.Up:
	//				if (index - 1 >= 0) {
	//					var item = ItemRoot.ItemsView.ElementAt(index - 1);

	//					SelectedItem.IsSelected = false;
	//					item.IsSelected = true;
	//					SelectionChangedHandler(item);
	//				}
	//				break;
	//			case Avalonia.Input.Key.Down:
	//				if (index + 1 < ItemRoot.ItemsView.Count) {
	//					var item = ItemRoot.ItemsView.ElementAt(index + 1);

	//					SelectedItem.IsSelected = false;
	//					item.IsSelected = true;
	//					SelectionChangedHandler(item);
	//				}
	//				break;
	//			default:
	//				break;
	//		}
	//	}
	//}

	private bool OnViewFilter(object? obj) {
		if (_filterItems == null) return true;

		return _filterItems.Invoke(obj);
	}

	public void UpdateFilter() {
		_itemsPresenter?.ItemRoot.UpdateView();
	}

	public void SelectionChangedHandler(TreeDataView2Item item) {
		List<TreeDataView2Item> removedItems = new();
		List<TreeDataView2Item> addedItems = new();

		if (SelectedItems.Contains(item)) {
			SelectedItems.Remove(item);
		}

		if (SelectionMode.HasFlag(SelectionMode.Single)) {
			foreach (var it in SelectedItems) {
				removedItems.Add(it);
				it.IsSelected = false;
			}
		}

		if (item.IsSelected) {
			addedItems.Add(item);
		}

		SelectedItems.Add(item);
		SelectedItems = new(SelectedItems.Where(it => it.IsSelected));
		SelectedItem = SelectedItems.FirstOrDefault();

		SelectionChangedEventArgs e = new(TreeDataView2.SelectionChangedEvent, removedItems, addedItems);
		RaiseEvent(e);
	}
}