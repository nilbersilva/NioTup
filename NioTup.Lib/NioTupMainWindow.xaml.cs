using NioTup.Lib.Classes;
using NioTup.Lib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NioTup.Lib
{
    /// <summary>
    /// Interaction logic for NioTupMainWindow.xaml
    /// </summary>
    public partial class NioTupMainWindow : Window
    {
        public NioTupMainWindow()
        {
            Shared.AddTheme();

            InitializeComponent();
            Shared.MainWindow = this;
            //string uriSource = "pack://application:,,,/" + typeof(NioTupMainWindow).Assembly.GetName().Name + ";component/Themes/ControlsDefaultTheme.xaml";
            //Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(uriSource, UriKind.RelativeOrAbsolute) });

            if (string.IsNullOrWhiteSpace(Shared.InstallVM.SetupInfo.LicenceUrlNavigate))
            {
                Shared.InstallVM.SetupInfo.LicenceUrlNavigate = Shared.InternalUrlShowLicensingFile;
            }

            if (Shared.InstallVM.SetupInfo.SetupDefaultFontSize == 0) Shared.InstallVM.SetupInfo.SetupDefaultFontSize = 14;

            if (!string.IsNullOrWhiteSpace(Shared.InstallVM.SetupInfo.DefaultInstallPath))
            {
                Shared.InstallVM.SetupInfo.DefaultInstallPath = Shared.HandleSpecialTags(Shared.InstallVM.SetupInfo.DefaultInstallPath);
            }

            Closed += nioTupMainWindow_Closed;
        }

        private void nioTupMainWindow_Closed(object sender, EventArgs e)
        {
            Shared.RemoveTheme();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {   
            this.DataContext = Shared.InstallVM;


            Shared.CallEvent(SetupEventNames.ApplicationStart, null, null); 

            if (Shared.InstallVM.SetupInfo.IgnoreWelcomeView)
            {
                Shared.InstallVM.MoveNext();
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            //Shared.InstallVM.SetupInfo.DarkTheme = !Shared.InstallVM.SetupInfo.DarkTheme;
            Shared.ChangeBaseTheme(Shared.InstallVM.SetupInfo.DarkTheme);
        }
    }
}
