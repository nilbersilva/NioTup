using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NioTup.Lib.Models
{
    public class SetupBaseConfig : BaseNotify
    {
        private string _ApplicationName;
        public string ApplicationName
        {
            get
            {
                return _ApplicationName;
            }
            set
            {
                _ApplicationName = value;
                Notify();
            }
        }

        private string _LicenceUrlNavigate;
        public string LicenceUrlNavigate
        {
            get
            {
                return _LicenceUrlNavigate;
            }
            set
            {
                _LicenceUrlNavigate = value;
                Notify();
            }
        }

        private string _CompanyName;
        public string CompanyName
        {
            get
            {
                return _CompanyName;
            }
            set
            {
                _CompanyName = value;
                Notify();
            }
        }

        private string _MainWindowTitle;
        public string MainWindowTitle
        {
            get
            {
                return _MainWindowTitle;
            }
            set
            {
                _MainWindowTitle = value;
                Notify();
            }
        }
        
        private string _DefaultInstallPath;
        public string DefaultInstallPath
        {
            get
            {
                return _DefaultInstallPath;
            }
            set
            {
                _DefaultInstallPath = value;
                Notify();
            }
        }

        private bool _RequiresDefaultInstallPath;
        public bool RequiresDefaultInstallPath
        {
            get
            {
                return _RequiresDefaultInstallPath;
            }
            set
            {
                _RequiresDefaultInstallPath = value;
                Notify();
            }
        }

        private bool _AutoCloseAfterInstall;
        public bool AutoCloseAfterInstall
        {
            get
            {
                return _AutoCloseAfterInstall;
            }
            set
            {
                _AutoCloseAfterInstall = value;
                Notify();
            }
        }

        private string _LicenceURL;
        public string LicenceURL
        {
            get
            {
                return _LicenceURL;
            }
            set
            {
                _LicenceURL = value;
                Notify();
            }
        }

        private double _SetupDefaultFontSize = 14;
        public double SetupDefaultFontSize
        {
            get
            {
                return _SetupDefaultFontSize;
            }
            set
            {
                _SetupDefaultFontSize = value;
                Notify();
            }
        }

        private bool _IgnoreWelcomeView;
        public bool IgnoreWelcomeView
        {
            get
            {
                return _IgnoreWelcomeView;
            }
            set
            {
                _IgnoreWelcomeView = value;
                Notify();
            }
        }

        private bool _DarkTheme = true;
        public bool DarkTheme
        {
            get
            {
                return _DarkTheme;
            }
            set
            {
                _DarkTheme = value;
                Notify();
            }
        }

        private string _AssemblyVersion;
        public string AssemblyVersion
        {
            get
            {
                return _AssemblyVersion;
            }
            set
            {
                _AssemblyVersion = value;
                Notify();
            }
        }

        private string _AssemblyFileVersion;
        public string AssemblyFileVersion
        {
            get
            {
                return _AssemblyFileVersion;
            }
            set
            {
                _AssemblyFileVersion = value;
                Notify();
            }
        }

        private string _AssemblyProduct;
        public string AssemblyProduct
        {
            get
            {
                return _AssemblyProduct;
            }
            set
            {
                _AssemblyProduct = value;
                Notify();
            }
        }

        private string _AssemblyTitle;
        public string AssemblyTitle
        {
            get
            {
                return _AssemblyTitle;
            }
            set
            {
                _AssemblyTitle = value;
                Notify();
            }
        }

        private string _AssemblyCopyright;
        public string AssemblyCopyright
        {
            get
            {
                return _AssemblyCopyright;
            }
            set
            {
                _AssemblyCopyright = value;
                Notify();
            }
        }
        
        private string _AssemblyDescription;
        public string AssemblyDescription
        {
            get
            {
                return _AssemblyDescription;
            }
            set
            {
                _AssemblyDescription = value;
                Notify();
            }
        }
        public List<SetupDownloadFile> DownloadFiles { get; set; }
    }
}
