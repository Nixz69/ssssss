using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2Test : UserControl {
	public TreeDataView2Test() {
		InitializeComponent();

		TreeDataView2Control.ItemsSource = new List<TestModel>() {
			new() {
				Text = "Text 1",
				Desc = "Desc 1",
				Additional = "Additional 1",
			},
			new() {
				Text = "Text 2",
				Desc = "Desc 2",
				Additional = "Additional 2",

				items = {
					new() {
						Text = "Text 1.1",
						Desc = "Desc 1.1",
						Additional = "Additional 1.1",
					},
					new() {
						Text = "Text 1.2",
						Desc = "Desc 1.2",
						Additional = "Additional 1.2",

						items = {
							new() {
								Text = "Text 2.3",
								Desc = "Desc 2.3",
								Additional = "Additional 2.3",
							},
							new() {
								Text = "Text 2.4",
								Desc = "Desc 2.4",
								Additional = "Additional 2.4",
							},
							new() {
								Text = "Text 2.5",
								Desc = "Desc 2.5",
								Additional = "Additional 2.5",
							},
						}
					},
					new() {
						Text = "Text 1.3",
						Desc = "Desc 1.3",
						Additional = "Additional 1.3",
					},
					new() {
						Text = "Text 1.4",
						Desc = "Desc 1.4",
						Additional = "Additional 1.4",
					},
					new() {
						Text = "Text 1.5",
						Desc = "Desc 1.5",
						Additional = "Additional 1.5",
					},
				}
			},
			new() {
				Text = "Text 3",
				Desc = "Desc 3",
				Additional = "Additional 3",
			},
			new() {
				Text = "Text 4",
				Desc = "Desc 4",
				Additional = "Additional 4",
			},
			new() {
				Text = "Text 5",
				Desc = "Desc 5",
				Additional = "Additional 5",
			},
		};
	}
}

public class TestModel : ReactiveObject {
	public string Text { get; set; } = string.Empty;

	public string Desc { get; set; } = string.Empty;

	public string Additional { get; set; } = string.Empty;

	public ObservableCollection<TestModel> items { get; set; } = new();
}