using MahApps.Metro.Controls;
using Microsoft.CodeAnalysis;
using NioTup.Lib;
using NioTup.Lib.ViewModels;
using NioTupApp.Models;
using NioTupApp.ViewModels;
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
        private MainWindowVM MyVM = new MainWindowVM();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = MyVM;
            MyVM.ProjectPath = "..\\..\\..\\..\\TestData\\TestName.niojson";

            if (File.Exists(MyVM.ProjectPath))
            {
                MyVM.CurrentSetup = System.Text.Json.JsonSerializer.Deserialize<SetupConfig>(File.ReadAllText(MyVM.ProjectPath));
            }
        }


        private async void acBuild(object sender, RoutedEventArgs e)
        {
           
            if (File.Exists(MyVM.ProjectPath))
            {
                if (MyVM.IsBusy) return;
                MyVM.IsBusy = true;
                try
                {
                    
                    MyVM.CurrentMessage = "Reading project file";
                    SetupConfig config = System.Text.Json.JsonSerializer.Deserialize<SetupConfig>(File.ReadAllText(MyVM.ProjectPath));

                    File.WriteAllText(MyVM.ProjectPath, JsonSerializer.Serialize(config, new JsonSerializerOptions() { WriteIndented = true }));

                    MyVM.CompilerLog = null;
                    MyVM.CurrentMessage = null;
                    
                   await Task.Run(async () =>
                    {
                        var compiler = new NioTupCompiler() { Debug = MyVM.IsDebugging };

                        compiler.LogMessage = x =>
                        {
                            MyVM.CompilerLog += x + "\n";
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                txtLog.ScrollToEnd();
                            }));
                        };
                        compiler.CurrentMessage = x =>
                        {
                            MyVM.CurrentMessage = x;
                        };

                        await compiler.Compile(config, MyVM.ProjectPath);
                    });
                }
                catch (Exception ex)
                {
                    Shared.ShowError(ex.Message + "\n" + ex.InnerException?.Message);
                }
                finally
                {
                    MyVM.IsBusy = false;
                }
            }
            else
            {
                MyVM.CurrentMessage = $"File not found: {MyVM.ProjectPath}";
            }
        }

        private void acShowSetupMainWindow(object sender, RoutedEventArgs e)
        {
            if (MyVM.IsBusy) return;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-GB");
            NioTup.Lib.Shared.InstallVM = new NioTupInstallVM()
            {
                SetupInfo = new NioTup.Lib.Models.SetupInfo()
                {
                    ApplicationName = "TEST APP",
                    CompanyName = "TEST COMPANY",
                    MainWindowTitle = "TEST MAIN WINDOW TITLE",
                    IgnoreWelcomeView = true,
                    DefaultInstallPath = "{ProgramFiles}\\TestApp",

                }
            };
            NioTupMainWindow win = new();
            win.Owner = this;
            win.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void acSelectFolder(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog
            {
                Title = "Select Project",
                Filter = "NioTup Project (*.niojson) |*.niojson|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MyVM.ProjectPath = dialog.FileName;
                try
                {
                    MyVM.CurrentSetup = System.Text.Json.JsonSerializer.Deserialize<SetupConfig>(File.ReadAllText(MyVM.ProjectPath));
                }
                catch (Exception ex)
                {
                    Shared.ShowError(ex.Message + "\n" + ex.InnerException?.Message);
                }                
            }
        }
    }


}

