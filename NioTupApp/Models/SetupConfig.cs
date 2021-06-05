using Microsoft.CodeAnalysis;
using NioTup.Lib.Models;
using NioTupApp.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NioTupApp.Models
{
    public class SetupConfig : SetupBaseConfig
    {
        private string _ApplicationAssemblyName;
        public string ApplicationAssemblyName
        {
            get
            {
                return _ApplicationAssemblyName;
            }
            set
            {
                _ApplicationAssemblyName = value;
                Notify();
            }
        }

        private string _ApplicationIconPath;
        public string ApplicationIconPath
        {
            get
            {
                return _ApplicationIconPath;
            }
            set
            {
                _ApplicationIconPath = value;
                Notify();
            }
        }

        private string _SplashImagePath;
        public string SplashImagePath
        {
            get
            {
                return _SplashImagePath;
            }
            set
            {
                _SplashImagePath = value;
                Notify();
            }
        }

        private string _CompanyLogoPath;
        public string CompanyLogoPath
        {
            get
            {
                return _CompanyLogoPath;
            }
            set
            {
                _CompanyLogoPath = value;
                Notify();
            }
        }

        private string _OutputPath;
        public string OutputPath
        {
            get
            {
                return _OutputPath;
            }
            set
            {
                _OutputPath = value;
                Notify();
            }
        }


        private string _LicencePath;
        public string LicencePath
        {
            get
            {
                return _LicencePath;
            }
            set
            {
                _LicencePath = value;
                Notify();
            }
        }

        private Platform _Platform = Platform.X64;
        public Platform Platform
        {
            get
            {
                return _Platform;
            }
            set
            {
                _Platform = value;
                Notify();
            }
        }

        private CompressionLevel _CompressionLevel = CompressionLevel.Optimal;
        public CompressionLevel CompressionLevel
        {
            get
            {
                return _CompressionLevel;
            }
            set
            {
                _CompressionLevel = value;
                Notify();
            }
        }

        private enNioTupFramework _Framework = enNioTupFramework.NETCore;
        public enNioTupFramework Framework
        {
            get
            {
                return _Framework;
            }
            set
            {
                _Framework = value;
                Notify();
            }
        }

        private ObservableCollection<SetupFiles> _Files  = new ObservableCollection<SetupFiles>();
        public ObservableCollection<SetupFiles> Files
        {
            get
            {
                return _Files;
            }
            set
            {
                _Files = value;
                Notify();
            }
        }

        private List<SetupDataKeys> _Keys;
        public List<SetupDataKeys> Keys
        {
            get
            {
                return _Keys;
            }
            set
            {
                _Keys = value;
                Notify();
            }
        }

        private string _SetupEventHandleScripts;
        public string SetupEventHandleScripts
        {
            get
            {
                return _SetupEventHandleScripts;
            }
            set
            {
                _SetupEventHandleScripts = value;
                Notify();
            }
        }

        private string _UserScripts;
        public string UserScripts
        {
            get
            {
                return _UserScripts;
            }
            set
            {
                _UserScripts = value;
                Notify();
            }
        }

        private List<string> _Languages;
        public List<string> Languages
        {
            get
            {
                return _Languages;
            }
            set
            {
                _Languages = value;
                Notify();
            }
        }

        private bool _SaveConfigAfterBuild;
        public bool SaveConfigAfterBuild
        {
            get
            {
                return _SaveConfigAfterBuild;
            }
            set
            {
                _SaveConfigAfterBuild = value;
                Notify();
            }
        }
    }

    public class SetupFiles
    {
        public List<SetupFile> From { get; set; }
        public List<string> To { get; set; }
    }

    public class SetupFile
    {
        public string GroupName { get; set; }
        public string FileName { get; set; }
        public string DestName { get; set; }
        public string ShortcutDestName { get; set; }
        public bool TopDirectoryOnly { get; set; }
    }
}
