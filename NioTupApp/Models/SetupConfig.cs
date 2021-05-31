using Microsoft.CodeAnalysis;
using NioTup.Lib.Models;
using NioTupApp.Enums;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NioTupApp.Models
{
    public class SetupConfig : SetupBaseConfig
    {
        public string ApplicationAssemblyName { get; set; }
        public string ApplicationIconPath { get; set; }
        public string SplashImagePath { get; set; }
        public string CompanyLogoPath { get; set; }
        public string OutputPath { get; set; }
        public string LicencePath { get; set; }
        public Platform Platform { get; set; } = Platform.X64;
        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;
        public enNioTupFramework Framework { get; set; } = enNioTupFramework.NETCore;
        public List<SetupFiles> Files { get; set; }
        public List<SetupDataKeys> Keys { get; set; }

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
