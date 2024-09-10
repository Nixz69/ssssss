using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia;
using Tmds.DBus.Protocol;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System;
using System.Data;
using System.Collections.ObjectModel;
using AS_Desktop2.UserControls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyAvaloniaApp;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Years();

            List<Enterprice> enterprices = new() {
                new() { name = "Всего ЗФ", 
                items = {
                        new() { name = "Горнорудные - директория", 
                        items = {
                                new() { name = "Горнорудные - группа",
                                items = {
                                    new() { name = "ЦГБ", },
                                    new() { name = "р. Комсомольский", },
                                    new() { name = "р. Маяк", },
                                    new() { name = "р. Октябрьский", },
                                    new() { name = "р. Таймырский", },
                                    }
                                },
                                new() { name = "Горнорудные 2 - группа",
                                items = {
                                    new() { name = "ЦГБ", },
                                    new() { name = "р. Комсомольский", },
                                    new() { name = "р. Маяк", },
                                    new() { name = "р. Октябрьский", },
                                    new() { name = "р. Таймырский", },
                                    }
                                }     
                            },                   
                        }
                    }
                },
            };

            treeDataView2.ItemsSource = enterprices;
        }
        private void Years()
        {
            var currentYear = DateTime.Now.Year;
            for (int year = currentYear; year <= 2050; year++)
            {
                YearComboBox.Items.Add(year);
            }
            //YearComboBox.SelectedIndex = 0; // Установить выбранный элемент по умолчанию
        }
        
       
        public class Enterprice : INotifyPropertyChanged
        {  
            //  private bool _isExpanded = false;

            
            public string name {get;set;} = "";
            public double jan {get; set;} = 1;
            public double feb {get; set;} = 2;
            public double mar {get; set;} = 3;
            public double apr {get; set;} = 4;
            public double may {get; set;} = 5;
            public double jun {get; set;} = 6;
            public double jul {get; set;} = 7;
            public double aug {get; set;} = 8;
            public double sep {get; set;} = 9;
            public double oct {get; set;} = 10;
            public double nov {get; set;} = 11;
            public double dec {get; set;} = 12;
            public double kw1 {get; set;} = new Random().Next();
            public double kw2 {get; set;} = new Random().Next();
            public double kw3 {get; set;} = new Random().Next();
            public double kw4 {get; set;} = new Random().Next();
            public double year {get; set;} = new Random().Next();

            // public bool IsExpanded {
            //     get => _isExpanded;
            //     set {
            //         _isExpanded = value;
            //         OnPropertyChanged(nameof(IsExpanded));
            //     }
            // }

            public ObservableCollection<Enterprice> items{get; set;} = new();

            public event PropertyChangedEventHandler? PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string? name = "") =>
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
        }

        private void BKW1_Click(object sender, RoutedEventArgs e)
        {
            if (BKW1.IsChecked == true)
            {
                Arrow_1.Source = new Avalonia.Media.Imaging.Bitmap("C:/Users/maksi/MyAvaloniaApp/Com_left.png");
            }
            else
            {
                Arrow_1.Source = new Avalonia.Media.Imaging.Bitmap("C:/Users/maksi/MyAvaloniaApp/Com_down.png");
            }
        }
        private void BKW2_Click(object sender, RoutedEventArgs e)
        {
            if (BKW2.IsChecked == true)
            {
                Arrow_2.Source = new Avalonia.Media.Imaging.Bitmap("C:/Users/maksi/MyAvaloniaApp/Com_left.png");
            }
            else
            {
                Arrow_2.Source = new Avalonia.Media.Imaging.Bitmap("C:/Users/maksi/MyAvaloniaApp/Com_down.png");
            }
        }
        private void BKW3_Click(object sender, RoutedEventArgs e)
        {
            if (BKW3.IsChecked == true)
            {
                Arrow_3.Source = new Avalonia.Media.Imaging.Bitmap("C:/Users/maksi/MyAvaloniaApp/Com_left.png");
            }
            else
            {
                Arrow_3.Source = new Avalonia.Media.Imaging.Bitmap("C:/Users/maksi/MyAvaloniaApp/Com_down.png");
            }
        }
        private void BKW4_Click(object sender, RoutedEventArgs e)
        {
            if (BKW4.IsChecked == true)
            {
                Arrow_4.Source = new Avalonia.Media.Imaging.Bitmap("C:/Users/maksi/MyAvaloniaApp/Com_left.png");
            }
            else
            {
                Arrow_4.Source = new Avalonia.Media.Imaging.Bitmap("C:/Users/maksi/MyAvaloniaApp/Com_down.png");
            }
        }

        private void Prim(object sender, RoutedEventArgs e)
        {
            if (planComboBox.SelectedItem != null && resourceComboBox.SelectedItem != null && contraAgentComboBox.SelectedItem != null)
            {
                // Приводим выбранный элемент к ComboBoxItem
                ComboBoxItem selectedItem = (ComboBoxItem)planComboBox.SelectedItem;
                ComboBoxItem selectedResource = (ComboBoxItem)resourceComboBox.SelectedItem;
                ComboBoxItem selectedContraAgent = (ComboBoxItem)contraAgentComboBox.SelectedItem;
                
                // Проверяем, что это именно "Текущий план"
                if (selectedItem.Name == "tekyshiy_plan" && selectedResource.Name == "obshiy_Resource" && selectedContraAgent.Name == "NMZ_Par")
                {
                    Gornorud.IsVisible = true;  // Показываем Gornorud
                }
            }
        }


        private void btn1(object sender, RoutedEventArgs e) 
        {
            Plan.IsVisible=true;
            Contrl.IsVisible=false;
            Otchet_1.IsVisible=false;
            Otchet_2.IsVisible=false;
            Otchet_3.IsVisible=false;
            Parametr.IsVisible=false;
        }

        private void btn2(object sender, RoutedEventArgs e)
        {
            Plan.IsVisible=false;
            Contrl.IsVisible=true;
            Otchet_1.IsVisible=false;
            Otchet_2.IsVisible=false;
            Otchet_3.IsVisible=false;
            Parametr.IsVisible=false;
        }
        private void btn3(object sender, PointerPressedEventArgs e)
        {
            
            Plan.IsVisible=false;
            Contrl.IsVisible=false;
            Otchet_1.IsVisible=true;
            Otchet_2.IsVisible=false;
            Otchet_3.IsVisible=false;
            Parametr.IsVisible=false;
        }
        private void btn4(object sender, PointerPressedEventArgs e)
        {
            
            Plan.IsVisible=false;
            Contrl.IsVisible=false;
            Otchet_1.IsVisible=false;
            Otchet_2.IsVisible=true;
            Otchet_3.IsVisible=false;
            Parametr.IsVisible=false;
        }
        private void btn5(object sender, PointerPressedEventArgs e)
        {
            Plan.IsVisible=false;
            Contrl.IsVisible=false;
            Otchet_1.IsVisible=false;
            Otchet_2.IsVisible=false;
            Otchet_3.IsVisible=true;
            Parametr.IsVisible=false;
        }
        private void btn6(object sender, RoutedEventArgs e)
        {
            Plan.IsVisible=false;
            Contrl.IsVisible=false;
            Otchet_1.IsVisible=false;
            Otchet_2.IsVisible=false;
            Otchet_3.IsVisible=false;
            Parametr.IsVisible=true;
        }


        
            
    }



