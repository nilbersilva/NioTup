using NioTup.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NioTup.Lib.Classes;

namespace NioTupApp
{
    public static class NioTupUserScripts
    {    

        public static void NioTupUserScriptsInit()
        {
            Shared.SetupEvent += SetupEventHandler;
        }

        private static void SetupEventHandler(string eventName, string action, string fileName)
        {
            //#SetupEventHandleScripts
        }

        //#UserScripts
    }
}

