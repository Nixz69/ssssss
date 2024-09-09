using Avalonia;
using Avalonia.Controls;
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
	private IEnumerable? _itemsSource;
	private TreeDataView2ExpanderColumn? _expanderColumn;
	private List<TreeDataView2Item> _itemsViewSource = new();
	private TreeDataView2ItemsPresenter? _itemsPresenter;

	private event Func<object?, bool>? _filterItems;
	#endregion

	#region public
	public static readonly StyledProperty<SelectionMode> SelectionModeProperty =
		AvaloniaProperty.Register<TreeDataView2, SelectionMode>(nameof(SelectionMode), SelectionMode.Single);

	public static readonly StyledProperty<TreeDataView2Item?> SelectedItemProperty =
		AvaloniaProperty.Register<TreeDataView2, TreeDataView2Item?>(nameof(SelectedItem));

	public static readonly StyledProperty<List<TreeDataView2Item>> SelectedItemsProperty =
		AvaloniaProperty.Register<TreeDataView2, List<TreeDataView2Item>>(nameof(SelectedItems), new());

	public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
		RoutedEvent.Register<TreeDataView2, SelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

	public static readonly DirectProperty<TreeDataView2, IEnumerable?> ItemsSourceProperty =
		AvaloniaProperty.RegisterDirect<TreeDataView2, IEnumerable?>(nameof(ItemsSource), x => x.ItemsSource, (x, value) => x.ItemsSource = value);

	//public static readonly StyledProperty<ObservableCollection<TreeDataView2Column>> ColumnsProperty =
	//	AvaloniaProperty.Register<TreeDataView2, ObservableCollection<TreeDataView2Column>>(nameof(Columns));

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
		get => _itemsSource;
		set {
			_itemsSource = value;
			Dispatcher.UIThread.Post(UpdateSource, DispatcherPriority.Background);
		}
	}

	public ObservableCollection<TreeDataView2Column> Columns {
		//get => GetValue(ColumnsProperty);
		//set => SetValue(ColumnsProperty, value);
		get;
	} = new();

	public SelectionMode SelectionMode {
		get => GetValue(SelectionModeProperty);
		set => SetValue(SelectionModeProperty, value);
	}

	public event EventHandler<SelectionChangedEventArgs> SelectionChanged {
		add => AddHandler(SelectionChangedEvent, value);
		remove => RemoveHandler(SelectionChangedEvent, value);
	}

	public TreeDataView2Item? SelectedItem {
		get => GetValue(SelectedItemProperty);
		set => SetValue(SelectedItemProperty, value);
	}

	public List<TreeDataView2Item> SelectedItems {
		get => GetValue(SelectedItemsProperty);
		set => SetValue(SelectedItemsProperty, value);
	}
	#endregion

	public TreeDataView2() {
        InitializeComponent();

		Columns.CollectionChanged += Columns_CollectionChanged;
		// Columns = new();
	}

	private TreeDataView2Row? BuildRow(object? @object, INameScope scope) {
		TreeDataView2Row item = new() { };

		foreach (var column in Columns) 
			column.BuildCell(item, @object, scope);

		return item;
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
					if (it is TreeDataView2ExpanderColumn expander) {
						_expanderColumn = expander;
					}
					it.AppendToGrid(PART_GridHeaders);
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				if (e.OldItems == null)
					break;
				foreach (var it in e.OldItems.OfType<TreeDataView2Column>()) {
					it.RemoveFromGrid();
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
				TreeDataView2Column[] columns = PART_GridHeaders.Children.OfType<TreeDataView2Column>().ToArray();
				foreach (var it in columns) {
					it.RemoveFromGrid();
				}
				break;
			// TODO: остальные действия над коллекцией
			default:
				break;
		}

		if (_itemsPresenter != null && 
			_itemsPresenter.ItemRoot != null &&
			_itemsPresenter.ItemRoot.ItemsView != null &&
			_itemsPresenter.ItemRoot.ItemsView.Count > 0) {
			Reset();
		}
	}

	public void Reset() {
		_itemsPresenter = new();
		_itemsPresenter.PART_ItemsRepeater.ItemTemplate = new FuncDataTemplate(@object => @object != null, BuildRow);

		PART_BorderRows.Child = _itemsPresenter;
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

	private void UpdateSource() {
		if (_itemsPresenter == null) {
			Reset();
		}
		
		IBinding? innerItemBinding = null;
		IBinding? isExpandedBinding = null;

		if (_expanderColumn != null) {
			innerItemBinding = _expanderColumn.InnerItemsSource;
			isExpandedBinding = _expanderColumn.IsExpanded;
		}

		_itemsPresenter!.ItemRoot = new(_itemsSource, innerItemBinding, isExpandedBinding);
		_itemsPresenter!.ItemRoot.OnViewFilter += OnViewFilter;
	}

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