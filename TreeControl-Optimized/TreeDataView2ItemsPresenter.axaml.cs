using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2ItemsPresenter : UserControl {
	#region private
	#endregion

	#region public
	public static readonly StyledProperty<TreeDataView2ItemRoot> ItemRootProperty =
		AvaloniaProperty.Register<TreeDataView2, TreeDataView2ItemRoot>(nameof(ItemRoot));

	public TreeDataView2ItemRoot ItemRoot {
		get => GetValue(ItemRootProperty);
		set => SetValue(ItemRootProperty, value);
	}
	#endregion

	public TreeDataView2ItemsPresenter() {
        InitializeComponent();
    }
}