using NioTup.Lib.Classes;
using NioTup.Lib.Controls;
using NioTup.Lib.Models;
using NioTup.Lib.Properties;
using NioTup.Lib.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NioTup.Lib.ViewModels
{
    public class NioTupInstallVM : BaseNotify
    {
        public SetupInfo SetupInfo { get; set; }

        public List<string> FilesCreated { get; set; } = new List<string>();
        public List<string> FoldersCreated { get; set; } = new List<string>();

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
            }
        }

        private bool _LicenceHostIsVisible;
        public bool LicenceHostIsVisible
        {
            get
            {
                return _LicenceHostIsVisible;
            }
            set
            {
                _LicenceHostIsVisible = value;
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

        private string _SetupLog;
        public string SetupLog
        {
            get
            {
                return _SetupLog;
            }
            set
            {
                _SetupLog = value;
                Notify();
            }
        }

        public string ButtonNextText
        {
            get
            {
                return Resources.ButtonNextText;
            }
        }
        public string ButtonPreviousText
        {
            get
            {
                return Resources.ButtonPreviousText;
            }
        }

        private bool _SelectFolderIsVisible = true;
        public bool SelectFolderIsVisible
        {
            get
            {
                return _SelectFolderIsVisible;
            }
            set
            {
                _SelectFolderIsVisible = value;
                Notify();
            }
        }

        private bool _SelectFolderIsEnabled = true;
        public bool SelectFolderIsEnabled
        {
            get
            {
                return _SelectFolderIsEnabled;
            }
            set
            {
                _SelectFolderIsEnabled = value;
                Notify();
            }
        }


        private bool _ButtonCancelIsVisible = true;
        public bool ButtonCancelIsVisible
        {
            get
            {
                return _ButtonCancelIsVisible;
            }
            set
            {
                _ButtonCancelIsVisible = value;
                Notify();
            }
        }

        private bool _ButtonPreviousIsVisible = false;
        public bool ButtonPreviousIsVisible
        {
            get
            {
                return _ButtonPreviousIsVisible;
            }
            set
            {
                _ButtonPreviousIsVisible = value;
                Notify();
            }
        }


        private bool _ButtonNextIsVisible = true;
        public bool ButtonNextIsVisible
        {
            get
            {
                return _ButtonNextIsVisible;
            }
            set
            {
                _ButtonNextIsVisible = value;
                Notify();
            }
        }

        private bool _CustomButtonIsVisible = false;
        public bool CustomButtonIsVisible
        {
            get
            {
                return _CustomButtonIsVisible;
            }
            set
            {
                _CustomButtonIsVisible = value;
                Notify();
            }
        }

        private string _CustomButtonText;
        public string CustomButtonText
        {
            get
            {
                return _CustomButtonText;
            }
            set
            {
                _CustomButtonText = value;
                Notify();
            }
        }

        private bool _LicenceAccepted = false;
        public bool LicenceAccepted
        {
            get
            {
                return _LicenceAccepted;
            }
            set
            {
                _LicenceAccepted = value;
                Notify();
                Notify("CommandNext");
            }
        }

        private ICommand _CommandNext;
        public ICommand CommandNext
        {
            get
            {
                return _CommandNext;
            }
            set
            {
                _CommandNext = value;
                Notify();
            }
        }
        public ICommand CommandPrevious { get; }
        public ICommand CommandCancel { get; }

        private MainView mainView = new();
        private LicensingAgreement licensingAgreement { get; set; }
        private FilesInstallView filesInstallView { get; set; }

        private object _MainWindowContent;
        public object MainWindowContent
        {
            get
            {
                return _MainWindowContent;
            }
            set
            {
                _MainWindowContent = value;
                Notify();
            }
        }

        public NioTupInstallVM()
        {
            MainWindowContent = mainView;
            CommandNext = new BaseCommand(async x =>
            {
                await MoveNext();
            }, (x) => CanMoveNext());
            CommandPrevious = new BaseCommand(x =>
            {
                MovePrevious();
            }, (x) => CanMovePrevious());
            CommandCancel = new BaseCommand(x =>
            {
                if (Shared.ShowQuestion(Properties.Resources.CancelExitSetup, Properties.Resources.Exit))
                {
                    Shared.MainWindow.Close();
                }
            }, (x) => CanCancel());
        }

        public void acLogAndSetupCurrentMessage(string msg)
        {
            CurrentMessage = msg;
            SetupLog += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + msg + Environment.NewLine;
        }

        public void acLog(string msg)
        {
            SetupLog += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + msg + Environment.NewLine;
        }

        internal async Task MoveNext()
        {
            if (MainWindowContent.GetType() == typeof(MainView))
            {
                //Move Licence
                if (licensingAgreement == null)
                {
                    licensingAgreement = new LicensingAgreement();
                }
                MainWindowContent = licensingAgreement;
                ButtonPreviousIsVisible = true;
                ButtonCancelIsVisible = false;


            }
            else if (MainWindowContent.GetType() == typeof(LicensingAgreement))
            {
                if (filesInstallView == null)
                {
                    filesInstallView = new FilesInstallView();
                }
                MainWindowContent = filesInstallView;
                ButtonCancelIsVisible = false;
                ButtonNextIsVisible = false;

                if (SelectFolderIsVisible)
                {
                    ButtonPreviousIsVisible = true;
                    CustomButtonText = Resources.Install;
                    CustomButtonIsVisible = true;
                }
                else
                {
                    await MoveNext();
                }
            }
            else if (MainWindowContent.GetType() == typeof(FilesInstallView))
            {

                if (SelectFolderIsVisible && string.IsNullOrWhiteSpace(this.SetupInfo.DefaultInstallPath))
                {
                    Shared.ShowError(Properties.Resources.PleaseDefineAPath);
                }
                else if (SelectFolderIsVisible && !PathHelper.ValidatePath(this.SetupInfo.DefaultInstallPath))
                {
                    Shared.ShowError(Properties.Resources.PleaseDefineAPath);
                }
                else
                {
                    SelectFolderIsEnabled = false;
                    ButtonPreviousIsVisible = false;
                    CustomButtonText = null;
                    CustomButtonIsVisible = false;
                    IsBusy = true;
                    await InstallSetup();
                }
            }
        }

        public bool CanMoveNext()
        {
            if (MainWindowContent.GetType() == typeof(LicensingAgreement))
            {
                if (!LicenceAccepted) return false;
            }
            return true;
        }

        private void MovePrevious()
        {
            if (MainWindowContent.GetType() == typeof(LicensingAgreement))
            {
                //Move Back to MainView
                MainWindowContent = mainView;
                ButtonPreviousIsVisible = false;
                ButtonCancelIsVisible = true;
            }
            else if (MainWindowContent.GetType() == typeof(FilesInstallView))
            {
                MainWindowContent = licensingAgreement;
                CustomButtonText = null;
                CustomButtonIsVisible = false;
                ButtonNextIsVisible = true;
            }
        }

        public bool CanMovePrevious()
        {
            return MainWindowContent.GetType() != typeof(MainView);
        }

        public bool CanCancel()
        {
            return MainWindowContent.GetType() == typeof(MainView) || MainWindowContent.GetType() == typeof(LicensingAgreement);
        }

        public async Task InstallSetup()
        {
            await Task.Run(async () =>
            {
                bool bError = false;
                try
                {

                    if (SetupInfo.DownloadFiles?.Count > 0)
                    {
                        Action<double> accaoReport = x =>
                        {
                            CurrentMessage = Properties.Resources.DownloadFile + ": " + x.ToString("F2", CultureInfo.CurrentUICulture) + "%";
                        };

                        foreach (var item in SetupInfo.DownloadFiles)
                        {
                            if (item.ReportProgress)
                            {
                                acLogAndSetupCurrentMessage(Properties.Resources.DownloadFile + ": " + Path.GetFileName(item.Destination));
                            }

                            string ToPath = Shared.HandleSpecialTags(item.Destination);
                            var directory = Path.GetDirectoryName(ToPath);

                            if (!Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                                FoldersCreated.Add(directory);
                            }

                            if (await Shared.FnDownloadAsync(item.URL, ToPath, item.Headers, item.ReportProgress ? accaoReport : null))
                            {
                                FilesCreated.Add(ToPath);
                                if (item.ReportProgress)
                                {
                                    acLogAndSetupCurrentMessage(Properties.Resources.DownloadComplete);
                                }
                            }
                        }
                    }

                    acLogAndSetupCurrentMessage(Properties.Resources.ValidatingFiles);
                    var str = Shared.GetEmbeddedResource("files.zip");

                    double bytes = 0;
                    if (str != null)
                    {
                        using (str)
                        using (var zip = new ZipArchive(str, ZipArchiveMode.Read))
                        {
                            string FileToPath = null;
                            foreach (var file in zip.Entries)
                            {
                                var folderindex = int.Parse(file.FullName.Split('/', StringSplitOptions.RemoveEmptyEntries).First());
                                string fileName = file.Name;

                                string sFolderTo = Shared.HandleSpecialTags(Shared.SetupData.Files[folderindex - 1].Destination);

                                if (!Directory.Exists(sFolderTo))
                                {
                                    Directory.CreateDirectory(sFolderTo);
                                    FoldersCreated.Add(sFolderTo);
                                }

                                FileToPath = System.IO.Path.Combine(sFolderTo, fileName);
                                acLog(string.Format(Properties.Resources.ExtractingFileTo, fileName, System.IO.Path.GetDirectoryName(sFolderTo)));

                                if (!UnZipFile(file, FileToPath))
                                {

                                }

                                bytes += file.Length;

                                FilesCreated.Add(FileToPath);
                            }
                        }
                    }

                    acLogAndSetupCurrentMessage(Properties.Resources.TotalSizeAfterExtract + " " + Shared.BytesToReadableText(bytes));


                }
                catch (Exception ex)
                {
                    bError = true;
                    Shared.ShowError(ex.Message + "\n" + ex.InnerException?.Message);
                }
                finally
                {
                    IsBusy = false;
                    if (!bError)
                    {
                        acLogAndSetupCurrentMessage(Properties.Resources.SetupCompleted);

                        if (Shared.SetupData.AutoCloseAfterInstall)
                        {
                            await Task.Delay(2000);
                            Shared.RunUI(() =>
                            {
                                Shared.MainWindow.Close();
                            });
                        }
                        else
                        {
                            Shared.ShowMessage(Properties.Resources.SetupCompleted);

                            CustomButtonText = Properties.Resources.Exit;
                            CustomButtonIsVisible = true;
                            CommandNext = new BaseCommand(x =>
                            {
                                Shared.MainWindow.Close();
                            }, (x) => true);
                        }
                    }
                }
            });
        }

        private bool UnZipFile(ZipArchiveEntry entry, string Destination)
        {
            try
            {
                entry.ExtractToFile(Destination, true);
                return true;
            }
            catch (Exception ex)
            {
                string msg = string.Format(Properties.Resources.NotPossibleToUnzipFile, Path.GetFileName(entry.Name), Destination) + "\n\n" + Properties.Resources.TryAgain;
                if (Shared.ShowQuestion(msg, null, true))
                {
                    return UnZipFile(entry, Destination);
                }
            }
            return false;
        }
    }
}
