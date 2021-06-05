using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NioTup.Lib.Classes
{
    public static class SetupEventActions
    {
        public static string DownloadStart => "DownloadStart";
        public static string DownloadComplete => "DownloadComplete";
        public static string ExtractStart => "ExtractStart";
        public static string ExtractComplete => "ExtractComplete";
    }
}
