using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.MSBuild;
using NioTup.Lib;
using NioTup.Lib.Models;
using NioTupApp.Models;
using NioTupApp.PlatformCompilers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NioTupApp
{
    internal class NioTupCompiler
    {
        public Action<string> LogMessage { get; set; }
        public Action<string> CurrentMessage { get; set; }
     
        public bool Debug { get; set; }
       

        public async Task<bool> Compile(SetupConfig config, string configPath)
        {
            if(config.Framework == Enums.enNioTupFramework.NETCore)
            {
                //Compile as WPF .NET Core 5.0 Application
                using(var compiler = new dotNetCoreCompiler() {  Debug = this.Debug, LogMessage = this.LogMessage, CurrentMessage  = this.CurrentMessage })
                {
                    return await compiler.Compile(config, configPath);
                }
            }
            else
            {
                throw new Exception("Framework not implemented yet");
            }
        }

    }
}
