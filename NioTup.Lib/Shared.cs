using CommandLine;
using NioTup.Lib.Models;
using NioTup.Lib.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static System.Environment;

namespace NioTup.Lib
{
    public static class Shared
    {
        internal static SetupInfo SetupData = null;
        public static NioTupInstallVM InstallVM = null;
        public static Assembly MainApplicationAssembly { get; set; }
        public static CommandLineOptions CommandLineParsed { get; set; }
        public static string InternalUrlShowLicensingFile { get; } = "https://niotup.lib";
        public static ResourceDictionary DefaultResourceColors { get; set; }
        public static Window MainWindow { get; set; }
        public static void AppStart()
        {
            if (MainApplicationAssembly == null) MainApplicationAssembly = Assembly.GetEntryAssembly();

            ParseCommandLine();



            using (var str = GetEmbeddedResource("setup.config.json"))
            {
                using (var reader = new StreamReader(str))
                {
                    SetupData = System.Text.Json.JsonSerializer.Deserialize<SetupInfo>(reader.ReadToEnd());
                }
            }

            if (string.IsNullOrWhiteSpace(SetupData.LicenceUrlNavigate))
            {
                SetupData.LicenceUrlNavigate = InternalUrlShowLicensingFile;
            }

            InstallVM = new NioTupInstallVM() { SetupInfo = SetupData, SelectFolderIsVisible = SetupData.RequiresDefaultInstallPath };

            if (InstallVM.SetupInfo.ApplicationName == null) InstallVM.SetupInfo.ApplicationName = MainApplicationAssembly.GetName().Name;



            //foreach (var key in resDic.Keys)
            //{
            //    //if (resDic[key.ToString()] is Color)
            //    //    resDic[key.ToString()] = Colors.DodgerBlue;
            //    if (key.ToString() == "ButtonBackgroundColorMouseOverColor")
            //    {
            //        resDic[key.ToString()] = Colors.DodgerBlue;
            //    }
            //}
            MainWindow = new NioTupMainWindow();
            Application.Current.MainWindow = MainWindow;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Application.Current.MainWindow.ShowDialog();
        }


        private static bool AddedTheme;
        internal static void AddTheme()
        {
            if (!AddedTheme)
            {
                DefaultResourceColors = new ResourceDictionary() { Source = new Uri("pack://application:,,,/NioTup.Lib;component/Themes/Defaults.xaml", UriKind.RelativeOrAbsolute) };
                ResourceDictionary resDicControls = new ResourceDictionary() { Source = new Uri("pack://application:,,,/NioTup.Lib;component/Themes/ControlsDefaultTheme.xaml", UriKind.RelativeOrAbsolute) };
                DefaultResourceColors.MergedDictionaries.Add(resDicControls);
                Application.Current.Resources.MergedDictionaries.Add(DefaultResourceColors);

                Shared.ChangeBaseTheme(Shared.InstallVM.SetupInfo.DarkTheme);

                AddedTheme = true;
            }
        }

        public static void RemoveTheme()
        {
            if (AddedTheme)
            {
                if (DefaultResourceColors != null)
                {
                    if (Application.Current.Resources.MergedDictionaries.Contains(DefaultResourceColors))
                    {
                        Application.Current.Resources.MergedDictionaries.Remove(DefaultResourceColors);
                    }
                }
                AddedTheme = false;
            }
        }

        internal static void ChangeBaseTheme(bool DarkTheme)
        {
            if (!DarkTheme)
            {
                Shared.DefaultResourceColors["Application.Brushes.Background.Color"] = Colors.White;
                Shared.DefaultResourceColors["Application.Brushes.Foreground.Color"] = Colors.Black;
                ((SolidColorBrush)Shared.DefaultResourceColors["Application.Brushes.Background"]).Color = Colors.White;
                ((SolidColorBrush)Shared.DefaultResourceColors["Application.Brushes.Foreground"]).Color = Colors.Black;
            }
            else
            {
                Shared.DefaultResourceColors["Application.Brushes.Background.Color"] = ColorConverter.ConvertFromString("#121214");
                Shared.DefaultResourceColors["Application.Brushes.Foreground.Color"] = Colors.White;
                ((SolidColorBrush)Shared.DefaultResourceColors["Application.Brushes.Background"]).Color = (Color)Shared.DefaultResourceColors["Application.Brushes.Background.Color"];
                ((SolidColorBrush)Shared.DefaultResourceColors["Application.Brushes.Foreground"]).Color = Colors.White;
            }
        }

        private static void ParseCommandLine()
        {
            if (Environment.GetCommandLineArgs().Length > 0)
            {
                var parser = new Parser((x) =>
                {
                    x.AutoHelp = true;
                    x.CaseSensitive = false;
                    x.IgnoreUnknownArguments = true;
                });

                var result = parser.ParseArguments<CommandLineOptions>(Environment.GetCommandLineArgs());
                result.MapResult((CommandLineOptions opts) => CommandLineParsed = opts,
                    (IEnumerable<Error> err) =>
                    {
                        return null;
                    }
                    );
            }
            else
            {
                //Empty
                CommandLineParsed = new CommandLineOptions();
            }
        }

        public static Stream GetEmbeddedResource(string aname)
        {
            if (MainApplicationAssembly == null) MainApplicationAssembly = Assembly.GetEntryAssembly();
            var names = MainApplicationAssembly.GetManifestResourceNames();
            return MainApplicationAssembly.GetManifestResourceStream(MainApplicationAssembly.GetName().Name + $".{aname}");
        }

        public static string HandleSpecialTags(string input_File_or_Directory)
        {
            string sOutput = input_File_or_Directory;

            foreach (var names in Enum.GetNames(typeof(SpecialFolder)))
            {
                string name = "{" + names + "}";
                if (sOutput.Contains(name))
                {
                    sOutput = sOutput.Replace(name, Environment.GetFolderPath(Enum.Parse<SpecialFolder>(names)));
                }
            }

            if (InstallVM != null)
            {
                if (InstallVM.SetupInfo.Keys?.Count > 0)
                {
                    foreach (var item in Shared.SetupData.Keys)
                    {
                        string name = "{" + item.Name + "}";
                        if (sOutput.Contains(name))
                        {
                            sOutput = sOutput.Replace(name, HandleSpecialTags(item.Value));
                        }
                    }
                }

                if (sOutput.Contains("{Default}"))
                {
                    sOutput = sOutput.Replace("{Default}", InstallVM.SetupInfo.DefaultInstallPath);
                }
            }



            return sOutput;
        }

        public static void CopyPropertiesTo<T, T2>(this T source, T2 dest)
        {
            var plist = from prop in typeof(T).GetProperties() where prop.CanRead && prop.CanWrite select prop;
            var propsDest = from prop in typeof(T2).GetProperties() where prop.CanRead && prop.CanWrite select prop;

            foreach (PropertyInfo prop in plist)
            {
                var propToSet = propsDest.Where(x => x.Name == prop.Name).FirstOrDefault();

                if (propToSet != null && prop.PropertyType == propToSet.PropertyType)
                {
                    propToSet.SetValue(dest, prop.GetValue(source));
                }
            }
        }

        public static void RunUI(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(action);
            }
        }

        public static void ShowMessage(string msg, string Title = null)
        {
            RunUI(() =>
            {
                if (string.IsNullOrWhiteSpace(Title)) Title = Properties.Resources.Information;
                Controls.NioTupMessageBox msgWindow = new Controls.NioTupMessageBox();
                msgWindow.MyVM.PathData = NioTupMessageBoxVM.InformationPath;
                msgWindow.MyVM.Message = msg;
                msgWindow.MyVM.Title = Title;
                msgWindow.MyVM.FirstButtonCommand = new BaseCommand((x) => msgWindow.Close(), null);
                msgWindow.Owner = Application.Current.MainWindow;
                msgWindow.ShowDialog();
            });
        }

        public static void ShowWarning(string msg, string Title = null)
        {
            RunUI(() =>
            {
                if (string.IsNullOrWhiteSpace(Title)) Title = Properties.Resources.Warning;
                Controls.NioTupMessageBox msgWindow = new Controls.NioTupMessageBox();
                msgWindow.MyVM.PathData = NioTupMessageBoxVM.WarningPath;
                msgWindow.MyVM.Message = msg;
                msgWindow.MyVM.Title = Title;
                msgWindow.MyVM.FirstButtonCommand = new BaseCommand((x) => msgWindow.Close(), null);
                msgWindow.Owner = Application.Current.MainWindow;
                msgWindow.ShowDialog();
            });
        }


        public static void ShowError(string msg, string Title = null)
        {
            RunUI(() =>
            {
                if (string.IsNullOrWhiteSpace(Title)) Title = Properties.Resources.Error;
                Controls.NioTupMessageBox msgWindow = new Controls.NioTupMessageBox();
                msgWindow.BorderBrush = Brushes.Red;
                msgWindow.MyVM.PathData = NioTupMessageBoxVM.ErrorPath;
                msgWindow.MyVM.Message = msg;
                msgWindow.MyVM.Title = Title;
                msgWindow.MyVM.FirstButtonCommand = new BaseCommand((x) => msgWindow.Close(), null);
                msgWindow.Owner = Application.Current.MainWindow;
                msgWindow.ShowDialog();
            });
        }

        public static bool ShowQuestion(string msg, string Title = null, bool defaultAwnser = false)
        {
            bool retValue = false;
            RunUI(() =>
            {
                if (string.IsNullOrWhiteSpace(Title)) Title = Properties.Resources.Question;
                Controls.NioTupMessageBox msgWindow = new();
                msgWindow.MyVM.PathData = NioTupMessageBoxVM.QuestionPath;
                msgWindow.MyVM.Message = msg;
                msgWindow.MyVM.Title = Title;
                msgWindow.MyVM.FirstButtonText = Properties.Resources.ButtonYesText;
                msgWindow.MyVM.SecondButtonIsVisible = true;
                msgWindow.MyVM.FirstButtonCommand = new BaseCommand((x) =>
                {
                    retValue = true;
                    msgWindow.Close();
                }, null);
                msgWindow.MyVM.SecondButtonCommand = new BaseCommand((x) =>
                {
                    retValue = false;
                    msgWindow.Close();
                }, null);
                msgWindow.Owner = Application.Current.MainWindow;
                void ContentRenderedHandler(object sender, EventArgs e)
                {
                    msgWindow.ContentRendered -= ContentRenderedHandler;
                    if (defaultAwnser)
                    {
                        msgWindow.btnFirst.Focus();
                    }
                    else
                    {
                        msgWindow.btnSecond.Focus();
                    }
                }
                msgWindow.ContentRendered += ContentRenderedHandler;

                msgWindow.ShowDialog();
            });
            return retValue;
        }

        public static string BytesToReadableText(double bytes)
        {
            string[] sizes = { "Bytes", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (bytes >= 1024 && order < sizes.Length - 1)
            {
                order++;
                bytes /= 1024;
            }

            return String.Format("{0:0.##} {1}", bytes, sizes[order]);
        }

        public static async Task<bool> FnDownloadAsync(string URL, string ToPath, Dictionary<string, string> Headers = null, Action<double> reportProgress = null)
        {
            try
            {
                URL = HandleSpecialTags(URL);
                ToPath = HandleSpecialTags(ToPath);

                var httpClientHandler = new System.Net.Http.HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli };
                using (var client = new HttpClient(httpClientHandler))
                {
                    if (Headers?.Count > 0)
                    {
                        foreach (var item in Headers)
                        {
                            client.DefaultRequestHeaders.Add(HandleSpecialTags(item.Key), HandleSpecialTags(item.Value));
                        }
                    }

                    var response = await client.GetAsync(new Uri(URL));

                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        var directory = Path.GetDirectoryName(ToPath);

                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        using (var fileStream = File.Open(ToPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            if (reportProgress != null && stream.Length > 0)
                            {
                                byte[] buffer = new byte[8192];
                                long size = stream.Length;
                                long downloaded = 0;
                                while (true)
                                {
                                    var length = await stream.ReadAsync(buffer, 0, buffer.Length);
                                    if (length <= 0) break;
                                    downloaded += length;

                                    await fileStream.WriteAsync(buffer, 0, length);

                                    reportProgress.Invoke((double)downloaded / size * 100);
                                }
                            }
                            else
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }
                    }

                   
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message + "\n" + ex.InnerException?.Message);
            }
            return false;
        }
    }
}
