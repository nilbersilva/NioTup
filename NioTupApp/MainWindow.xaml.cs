using MahApps.Metro.Controls;
using Microsoft.CodeAnalysis;
using NioTup.Lib;
using NioTup.Lib.ViewModels;
using NioTupApp.Models;
using NioTupApp.ViewModels;
using NioTupApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NioTupApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        internal static MainWindowVM MyVM = new MainWindowVM();

        private ProjectView projectView = new ProjectView();


        public MainWindow()
        {
            InitializeComponent();

            HamburgerMenuControl.SelectedItem = ((HamburgerMenuItemCollection)HamburgerMenuControl.ItemsSource).First();
            HamburgerMenuControl.Content = projectView;

            this.DataContext = MyVM;
            MyVM.ProjectPath = "..\\..\\..\\..\\TestData\\TestName.niojson";

            if (File.Exists(MyVM.ProjectPath))
            {
                MyVM.CurrentSetup = System.Text.Json.JsonSerializer.Deserialize<SetupConfig>(File.ReadAllText(MyVM.ProjectPath));
            }
        }              
    
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

       

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args)
        {
            if (args.InvokedItem is HamburgerMenuGlyphItem item)
            {
                if((string)item.Tag == "ShowProject")
                {
                    HamburgerMenuControl.Content = projectView;
                }
            }
        }
    }


}

