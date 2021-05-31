using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NioTup.Lib.Models
{
    public class SetupInfo : SetupBaseConfig
    {
      
        public List<SetupDataFile> Files { get; set; }
        public List<SetupDataKeys> Keys { get; set; }

        public bool HasLicence { get; set; }

    }

    public class SetupDataFile
    {
        public string Destination { get; set; }
    }

    public class SetupDataKeys
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class SetupDownloadFile
    {
        public string URL { get; set; }
        public Dictionary<string,string> Headers { get; set; }
        public string Destination { get; set; }
        public bool ReportProgress { get; set; }
    }
}
