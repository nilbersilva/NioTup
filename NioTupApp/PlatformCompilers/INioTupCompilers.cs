using NioTupApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NioTupApp.PlatformCompilers
{
    public interface INioTupCompilers : IDisposable
    {
        Action<string> LogMessage { get; set; }
        Action<string> CurrentMessage { get; set; }
        Task<bool> Compile(SetupConfig config, string configPath);
        bool Debug { get; set; }
    }
}
