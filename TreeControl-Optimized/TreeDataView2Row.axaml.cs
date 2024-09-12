using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Specialized;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2Row : UserControl {
	#region private
	private TreeDataView2? _root = null;
	#endregion

	#region public
	public static readonly StyledProperty<bool> IsSelectedProperty =
		AvaloniaProperty.Register<TreeDataView2Row, bool>(nameof(IsSelected), default, false, BindingMode.TwoWay);

	public bool IsSelected {
		get => GetValue(IsSelectedProperty);
		set => SetValue(IsSelectedProperty, value);
	}
	#endregion

	public TreeDataView2Row() {
		InitializeComponent();

		this[!IsSelectedProperty] = new Binding(nameof(IsSelected));

		PointerPressed += TreeDataView2Row_PointerPressed;
	}

	private TreeDataView2? GetRoot() {
		if (_root == null) {
			_root = this.FindLogicalAncestorOfType<TreeDataView2>();
		}
		return _root;
	}

	private void TreeDataView2Row_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e) {
		if (DataContext is not TreeDataView2Item item) return;

		TreeDataView2? root = GetRoot();
		if (root == null) return;

		bool bSelected = IsSelected;

		switch (e.ClickCount) {
			case 1:
				if (root.SelectionMode.HasFlag(SelectionMode.Toggle)) {
					bSelected ^= true;
				}
				else {
					bSelected = true;
				}

				if (root.SelectionMode.HasFlag(SelectionMode.AlwaysSelected)) {
					bSelected = true;
				}

				root.RaiseEvent(new ItemClickEventArgs(TreeDataView2.ClickRowEvent, this));

				e.Handled = true;
				break;
			case 2:
				if (item.IsHierarchical) {
					item.IsExpanded ^= true;

					bSelected = true;
				}

				root.RaiseEvent(new ItemClickEventArgs(TreeDataView2.DoubleClickRowEvent, this));

				e.Handled = true;
				break;
			default:
				break;
		}

		IsSelected = bSelected;

		root.SelectionChangedHandler(item);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
		if (change.Property == IsSelectedProperty) {
			// SelectionHandler
		}
		
		base.OnPropertyChanged(change);
	}
}

public class ItemClickEventArgs : RoutedEventArgs {
	public object? Item { get; set; } = null;

	public ItemClickEventArgs(RoutedEvent? routedEvent, object? item) {
		RoutedEvent = routedEvent;
		Item = item;
	}
}
