using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NioTup.Lib.Models
{
    public class CommandLineOptions
    {
        [Option('s', "silent", Default = false, HelpText = "Silent Install without showing MainWindow.")]
        public bool Silent { get; set; }


    }
}
