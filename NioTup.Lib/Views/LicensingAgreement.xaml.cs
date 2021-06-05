using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
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

namespace NioTup.Lib.Views
{
    /// <summary>
    /// Interaction logic for LicensingAgreement.xaml
    /// </summary>
    public partial class LicensingAgreement : UserControl
    {
        public LicensingAgreement()
        {
            InitializeComponent();

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (e.Uri.OriginalString == Shared.InternalUrlShowLicensingFile)
            {
                MessageBox.Show("SHOW LICENCE");
            }
            else
            {
                //Open Link
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(Window.GetWindow(this));
            
            if (Shared.InstallVM != null)
                if (Shared.InstallVM.SetupInfo.HasLicence)
                {
                    Task.Run(async () =>
                    {
                        var strLicence = Shared.GetEmbeddedResource("licence.zip");

                        if (strLicence != null)
                        {
                            using (strLicence)
                            using (var strUnzip = new GZipStream(strLicence, CompressionMode.Decompress))
                            //using (var memStream = new StreamReader(strUnzip))
                            {
                                //await strUnzip.CopyToAsync(memStream);
                                //memStream.Position = 0;
                                Shared.RunUI(async () =>
                                {
                                    rtb.Selection.Load(strUnzip, DataFormats.Rtf);
                                    //rtb.LoadFile(memStream, System.Windows.Forms.RichTextBoxStreamType.RichText);
                                });
                            }
                        }
                    });
                }
        }
    }
}
