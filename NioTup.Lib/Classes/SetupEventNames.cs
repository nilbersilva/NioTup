using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NioTup.Lib.Classes
{
    public static class SetupEventNames
    {
        public static string ApplicationStart => "ApplicationStarted";
        public static string MainContentChanged => "MainContentChanged";
        public static string ExtractingFile => "ExtractingFile";
        public static string DownloadFile => "DownloadFile";
        public static string ExtractFile => "ExtractFile";
    }
}
