using Microsoft.CodeAnalysis;
using NioTup.Lib;
using NioTup.Lib.ViewModels;
using NioTupApp.Enums;
using NioTupApp.Models;
using NioTupApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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

namespace NioTupApp.Views
{
    /// <summary>
    /// Interaction logic for ProjectView.xaml
    /// </summary>
    public partial class ProjectView : UserControl
    {
        MainWindowVM MyVM = MainWindow.MyVM;

        public ProjectView()
        {
            InitializeComponent();

            cmbPlatform.ItemsSource = new List<Platform>() { Platform.X64, Platform.X86 };
            cmbCompression.ItemsSource = new List<CompressionLevel>() { CompressionLevel.Optimal, CompressionLevel.Fastest };
            cmbFramework.ItemsSource = new List<enNioTupFramework>() { enNioTupFramework.NETCore };
        }

        private async void acBuild(object sender, RoutedEventArgs e)
        {

            if (File.Exists(MyVM.ProjectPath))
            {
                if (MyVM.IsBusy) return;
                MyVM.IsBusy = true;
                try
                {
                    MainTabControl.SelectedItem = CompilationLogTab;
                    MyVM.CurrentMessage = "Reading project file";

                    MyVM.CompilerLog = null;
                    MyVM.CurrentMessage = null;

                    await Task.Run(async () =>
                    {
                        var compiler = new NioTupCompiler() { Debug = MyVM.IsDebugging };

                        compiler.LogMessage = x =>
                        {
                            MyVM.CompilerLog += x + "\n";
                        };
                        compiler.CurrentMessage = x =>
                        {
                            MyVM.CurrentMessage = x;
                        };

                        if (await compiler.Compile(MyVM.CurrentSetup, MyVM.ProjectPath))
                        {
                            MyVM.CurrentMessage = null;
                            if (MyVM.CurrentSetup.SaveConfigAfterBuild)
                                File.WriteAllText(MyVM.ProjectPath, JsonSerializer.Serialize(MyVM.CurrentSetup, new JsonSerializerOptions() { WriteIndented = true }));
                        }
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
            win.Owner = Window.GetWindow(this);
            win.ShowDialog();
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

        private bool AutoScroll = true;
        private void TextBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var _scrollViewer = e.OriginalSource as ScrollViewer;
            // User scroll event : set or unset auto-scroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (_scrollViewer.VerticalOffset == _scrollViewer.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set auto-scroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset auto-scroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : auto-scroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and auto-scroll mode set
                // Autoscroll
                _scrollViewer.ScrollToVerticalOffset(_scrollViewer.ExtentHeight);
            }
        }
    }
}
