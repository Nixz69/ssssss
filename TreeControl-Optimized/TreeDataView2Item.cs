using Avalonia;
using Avalonia.Collections;
using Avalonia.Data;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AS_Desktop2.UserControls;

public class TreeDataView2Item : StyledElement {
	#region private
	private TreeDataView2ItemRoot _root;

	private AvaloniaList<TreeDataView2Item> _items = new();
	private bool _include = true;
	private TreeDataView2Item? _parent = null;
	#endregion

	#region public
	public new event PropertyChangedEventHandler? PropertyChanged;

	public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
		AvaloniaProperty.Register<TreeDataView2Item, IEnumerable?>(nameof(ItemsSource));

	public static readonly StyledProperty<AvaloniaList<TreeDataView2Item>> ItemsProperty =
		AvaloniaProperty.Register<TreeDataView2Item, AvaloniaList<TreeDataView2Item>>(nameof(Items));

	public static readonly StyledProperty<bool> IsExpandedProperty =
		AvaloniaProperty.Register<TreeDataView2Item, bool>(nameof(IsExpanded), default, false, BindingMode.TwoWay);

	public static readonly StyledProperty<bool> IsSelectedProperty =
		AvaloniaProperty.Register<TreeDataView2Item, bool>(nameof(IsSelected));

	public static readonly StyledProperty<int> LevelProperty =
		AvaloniaProperty.Register<TreeDataView2Item, int>(nameof(Level));

	public static readonly DirectProperty<TreeDataView2Item, object?> ContextProperty =
		AvaloniaProperty.RegisterDirect<TreeDataView2Item, object?>("Context", x => x.DataContext, (x, value) => x.DataContext = value);

	//public IBinding? ItemsSourceBinding {
	//	get => _itemsSourceBinding;
	//	set {
	//		_itemsSourceBinding = value;

	//		if (_itemsSourceBinding != null) {
	//			this[!ItemsSourceProperty] = _itemsSourceBinding;
	//		}
	//	}
	//}

	//public IBinding? IsExpandedBinding {
	//	get => _isExpandedBinding;
	//	set {
	//		_isExpandedBinding = value;

	//		if (_isExpandedBinding != null) {
	//			this[!IsExpandedProperty] = _isExpandedBinding;
	//		}
	//	}
	//}

	public IEnumerable? ItemsSource {
		get => GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	public bool IsExpanded {
		get => GetValue(IsExpandedProperty);
		set => SetValue(IsExpandedProperty, value);
	}

	public bool IsSelected {
		get => GetValue(IsSelectedProperty);
		set => SetValue(IsSelectedProperty, value);
	}

	public bool Include {
		get => _include;
		set {
			_include = value;

			if (_include) {
				if (_parent != null) {
					_parent.Include = _include;
				}
			}
		}
	}

	private int _level = 0;

	public int Level {
		get => GetValue(LevelProperty);
		set => SetValue(LevelProperty, value);
	}

	public bool IsHierarchical { get => _root.IsExpandedBinding != null; }

	public AvaloniaList<TreeDataView2Item> Items {
		get => GetValue(ItemsProperty);
		set => SetValue(ItemsProperty, value);
	}
	#endregion

	public TreeDataView2Item(TreeDataView2ItemRoot root, TreeDataView2Item? parent, object? value) {
		_root = root;
		_parent = parent;

		if (parent != null) {
			Level = parent.Level + 1;
		}

		if (_root.IsExpandedBinding != null) {
			this[!TreeDataView2Item.IsExpandedProperty] = _root.IsExpandedBinding;
		}

		if (_root.InnerItemsBinding != null) {
			this[!TreeDataView2Item.ItemsSourceProperty] = _root.InnerItemsBinding;
		}

		DataContext = value;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
		if (change.Property == ItemsSourceProperty) {
			InitiateItemsSource();
		}

		if (change.Property == IsExpandedProperty) {
			_root.UpdateView();
		}

		base.OnPropertyChanged(change);
	}

	private void InitiateItemsSource() {
		Items = new();

		if (ItemsSource != null) {
			foreach (var item in ItemsSource) {
				Items.Add(new(_root, this, item));
			}
		}

		_root.UpdateView();
	}
}

public class TreeDataView2ItemRoot : ReactiveObject {
	#region private
	private ObservableCollection<TreeDataView2Item> _itemsSource = new();
	private ObservableCollection<TreeDataView2Item> _itemsView = new();

	public event Action? OnViewUpdated;
	public event Func<object?, bool>? OnViewFilter;

	private IBinding? _innerItemsBinding = null;
	private IBinding? _isExpandedBinding = null;
	#endregion

	#region public
	public IBinding? InnerItemsBinding {
		get => _innerItemsBinding;
	}

	public IBinding? IsExpandedBinding {
		get => _isExpandedBinding;
	}

	public ObservableCollection<TreeDataView2Item> ItemsView {
		get => _itemsView;
		set => this.RaiseAndSetIfChanged(ref _itemsView, value);
	}
	#endregion

	public TreeDataView2ItemRoot(IEnumerable? source, IBinding? innerItemsBinding, IBinding? isExpandedBinding) {
		_innerItemsBinding = innerItemsBinding;
		_isExpandedBinding = isExpandedBinding;

		if (source != null) {
			foreach (var item in source) {
				_itemsSource.Add(new(this, null, item));
			}
		}
		
		UpdateView();
	}

	public void UpdateView() {
		List<TreeDataView2Item> items = new();

		foreach (var it in _itemsSource) {
			AppendItems(items, it);
		}

		if (OnViewFilter != null) {
			UpdateItemsViewWithFilter(items);
		}

		ItemsView = new(items);

		OnViewUpdated?.Invoke();
	}

	private void AppendItems(List<TreeDataView2Item> list, TreeDataView2Item node) {
		if (OnViewFilter != null) {
			if (OnViewFilter.Invoke(node.DataContext)) {
				node.Include = true;
			}
			else {
				node.Include = false;
			}
		}

		list.Add(node);

		if (node.IsExpanded)
			foreach (var ch in node.Items)
				AppendItems(list, ch);
	}

	private void UpdateItemsViewWithFilter(List<TreeDataView2Item> list) {
		for (int i = 0; i < list.Count;) {
			if (!list[i].Include) {
				list.RemoveAt(i);
				continue;
			}
			i++;
		}
	}

	public void Filter() => UpdateView();
}
