using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace NioTup.Lib.ViewModels
{
    public class NioTupMessageBoxVM : Models.BaseNotify
    {
        private string _FirstButtonText = Properties.Resources.OK;
        public string FirstButtonText
        {
            get
            {
                return _FirstButtonText;
            }
            set
            {
                _FirstButtonText = value;
                Notify();
            }
        }

        private string _SecondButtonText = Properties.Resources.ButtonNoText;
        public string SecondButtonText
        {
            get
            {
                return _SecondButtonText;
            }
            set
            {
                _SecondButtonText = value;
                Notify();
            }
        }

        private string _ThirdButtonText = Properties.Resources.ButtonCancelText;
        public string ThirdButtonText
        {
            get
            {
                return _ThirdButtonText;
            }
            set
            {
                _ThirdButtonText = value;
                Notify();
            }
        }

        private bool _FirstButtonIsVisible = true;
        public bool FirstButtonIsVisible
        {
            get
            {
                return _FirstButtonIsVisible;
            }
            set
            {
                _FirstButtonIsVisible = value;
                Notify();
            }
        }

        private bool _SecondButtonIsVisible;
        public bool SecondButtonIsVisible
        {
            get
            {
                return _SecondButtonIsVisible;
            }
            set
            {
                _SecondButtonIsVisible = value;
                Notify();
            }
        }

        private bool _ThirdButtonIsVisible;
        public bool ThirdButtonIsVisible
        {
            get
            {
                return _ThirdButtonIsVisible;
            }
            set
            {
                _ThirdButtonIsVisible = value;
                Notify();
            }
        }

        private ICommand _FirstButtonCommand;
        public ICommand FirstButtonCommand
        {
            get
            {
                return _FirstButtonCommand;
            }
            set
            {
                _FirstButtonCommand = value;
                Notify();
            }
        }

        private ICommand _SecondButtonCommand;
        public ICommand SecondButtonCommand
        {
            get
            {
                return _SecondButtonCommand;
            }
            set
            {
                _SecondButtonCommand = value;
                Notify();
            }
        }
        private ICommand _ThirdButtonCommand;
        public ICommand ThirdButtonCommand
        {
            get
            {
                return _ThirdButtonCommand;
            }
            set
            {
                _ThirdButtonCommand = value;
                Notify();
            }
        }

        private string _Title;
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                Notify();
            }
        }

        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
                Notify();
            }
        }

        public static string InformationPath = "M13.5,4A1.5,1.5 0 0,0 12,5.5A1.5,1.5 0 0,0 13.5,7A1.5,1.5 0 0,0 15,5.5A1.5,1.5 0 0,0 13.5,4M13.14,8.77C11.95,8.87 8.7,11.46 8.7,11.46C8.5,11.61 8.56,11.6 8.72,11.88C8.88,12.15 8.86,12.17 9.05,12.04C9.25,11.91 9.58,11.7 10.13,11.36C12.25,10 10.47,13.14 9.56,18.43C9.2,21.05 11.56,19.7 12.17,19.3C12.77,18.91 14.38,17.8 14.54,17.69C14.76,17.54 14.6,17.42 14.43,17.17C14.31,17 14.19,17.12 14.19,17.12C13.54,17.55 12.35,18.45 12.19,17.88C12,17.31 13.22,13.4 13.89,10.71C14,10.07 14.3,8.67 13.14,8.77Z";
        public static string QuestionPath = "M20 2H4C2.9 2 2 2.9 2 4V22L6 18H20C21.1 18 22 17.1 22 16V4C22 2.9 21.1 2 20 2M20 16H5.2L4 17.2V4H20V16M12.2 5.5C11.3 5.5 10.6 5.7 10.1 6C9.5 6.4 9.2 7 9.3 7.7H11.3C11.3 7.4 11.4 7.2 11.6 7.1C11.8 7 12 6.9 12.3 6.9C12.6 6.9 12.9 7 13.1 7.2C13.3 7.4 13.4 7.6 13.4 7.9C13.4 8.2 13.3 8.4 13.2 8.6C13 8.8 12.8 9 12.6 9.1C12.1 9.4 11.7 9.7 11.5 9.9C11.1 10.2 11 10.5 11 11H13C13 10.7 13.1 10.5 13.1 10.3C13.2 10.1 13.4 10 13.6 9.8C14.1 9.6 14.4 9.3 14.7 8.9C15 8.5 15.1 8.1 15.1 7.7C15.1 7 14.8 6.4 14.3 6C13.9 5.7 13.1 5.5 12.2 5.5M11 12V14H13V12H11Z";

        public static string WarningPath = "M13,10H11V6H13V10M13,12H11V14H13V12M22,4V16A2,2 0 0,1 20,18H6L2,22V4A2,2 0 0,1 4,2H20A2,2 0 0,1 22,4M20,4H4V17.2L5.2,16H20V4Z";

        public static string ErrorPath = "M11,15H13V17H11V15M11,7H13V13H11V7M12,2C6.47,2 2,6.5 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20Z";

        private string _PathData;
        [TypeConverter(typeof(Geometry))]
        public string PathData
        {
            get
            {
                return _PathData;
            }
            set
            {
                _PathData = value;
                Notify();
            }
        }

    }
}
