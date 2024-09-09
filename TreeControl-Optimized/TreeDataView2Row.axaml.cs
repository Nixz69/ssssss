using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;

using System.Collections.Specialized;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2Row : UserControl {
	#region private
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

	private void TreeDataView2Row_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e) {
		if (DataContext is not TreeDataView2Item item) return;
		
		TreeDataView2? root = this.FindLogicalAncestorOfType<TreeDataView2>();
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
				e.Handled = true;
				break;
			case 2:
				if (item.IsHierarchical) {
					item.IsExpanded ^= true;

					bSelected = true;
				}
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