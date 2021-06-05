using NioTup.Lib.Models;
using NioTupApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NioTupApp.ViewModels
{
   public class MainWindowVM : BaseNotify
    {
        private bool _IsBusy;
        public bool IsBusy
        {
            get
            {
                return _IsBusy;
            }
            set
            {
                _IsBusy = value;
                Notify();
                Notify("ControlsAreEnabled");
            }
        }


        public bool ControlsAreEnabled
        {
            get
            {
                return !IsBusy;
            }
        }

        private bool _IsDebugging;
        public bool IsDebugging
        {
            get
            {
                return _IsDebugging;
            }
            set
            {
                _IsDebugging = value;
                Notify();
            }
        }

        private string _CurrentMessage;
        public string CurrentMessage
        {
            get
            {
                return _CurrentMessage;
            }
            set
            {
                _CurrentMessage = value;
                Notify();
            }
        }

        private string _CompilerLog;
        public string CompilerLog
        {
            get
            {
                return _CompilerLog;
            }
            set
            {
                _CompilerLog = value;
                Notify();
            }
        }

        private string _ProjectPath;
        public string ProjectPath
        {
            get
            {
                return _ProjectPath;
            }
            set
            {
                _ProjectPath = value;
                Notify();
            }
        }

        private SetupConfig _CurrentSetup;
        public SetupConfig CurrentSetup
        {
            get
            {
                return _CurrentSetup;
            }
            set
            {
                _CurrentSetup = value;
                Notify();
            }
        }
    }
}
