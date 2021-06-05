using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NioTup.Lib;
using NioTup.Lib.Models;
using NioTupApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NioTupApp.PlatformCompilers
{
    public class dotNetCoreCompiler : INioTupCompilers
    {
        public Action<string> LogMessage { get; set; }
        public Action<string> CurrentMessage { get; set; }
        public bool Debug { get; set; }

        private readonly string PathID = Guid.NewGuid().ToString();

        public async Task<bool> Compile(SetupConfig config, string configPath)
        {

            Action<string> actionLogAndCurrentMessage = str =>
            {
                CurrentMessage?.Invoke(str);
                LogMessage?.Invoke(str);
            };

            try
            {
                var relPathCurrentConfig = Path.GetDirectoryName(Path.GetFullPath(configPath));
                SetupInfo setupData = new SetupInfo
                {
                    Files = new List<SetupDataFile>(),
                };

                //Copy Default Properties to Setup Config Data
                Shared.CopyPropertiesTo(config, setupData);

                string CompilationMainFolder = Path.Combine("./", "NioTupLib", PathID);
                Directory.CreateDirectory(CompilationMainFolder);

                string sFilesFolder = Path.Combine(CompilationMainFolder, "Compile");
                sFilesFolder = Path.GetFullPath(sFilesFolder);
                Directory.CreateDirectory(sFilesFolder);

                string projectFilePath = Path.Combine(sFilesFolder, $"{config.ApplicationAssemblyName}.csproj");

                //Fix Assembly Version and validation
                string AssemblyVersion = "";
                string AssemblyFileVersion = null;

                if (!string.IsNullOrWhiteSpace(config.AssemblyFileVersion))
                {
                    if (Version.TryParse(config.AssemblyFileVersion, out var version))
                    {

                        AssemblyVersion = version.ToString();
                    }
                    else
                    {
                        actionLogAndCurrentMessage($"Invalid AssemblyFileVersion: {config.AssemblyFileVersion}");
                        return false;
                    }
                }
                else
                {
                    AssemblyVersion = "1.0.0.0";
                }

                if (!string.IsNullOrWhiteSpace(config.AssemblyVersion))
                {
                    if (Version.TryParse(config.AssemblyVersion, out var version))
                    {

                        AssemblyFileVersion = version.ToString();
                    }
                    else
                    {
                        actionLogAndCurrentMessage($"Invalid AssemblyVersionAttribute: {config.AssemblyFileVersion}");
                        return false;
                    }
                }
                else
                {
                    AssemblyFileVersion = AssemblyVersion;
                }
              
             
                string LicenceCode = null;
                if (!string.IsNullOrEmpty(config.LicencePath))
                {
                    string licencePath = Path.Combine(relPathCurrentConfig, config.LicencePath);

                    if (File.Exists(licencePath))
                    {
                        LicenceCode = "licence.zip";
                        File.Delete(Path.Combine(sFilesFolder, LicenceCode));

                        using (var fileReader = File.Open(licencePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var destFile = File.Open(Path.Combine(sFilesFolder, LicenceCode), FileMode.OpenOrCreate, FileAccess.Write))
                        using (var zipLience = new GZipStream(destFile, CompressionLevel.Optimal))
                        {
                            await fileReader.CopyToAsync(zipLience);
                            setupData.HasLicence = true;
                        }
                        LicenceCode = $"<EmbeddedResource Include=\"{LicenceCode}\" />";
                    }
                    else
                    {
                        actionLogAndCurrentMessage($"Licence file not found: {config.SplashImagePath}");
                        return false;
                    }
                }
                
                string ApplicationSplashCode = null;
                if (!string.IsNullOrEmpty(config.SplashImagePath))
                {
                    string SplashImagePath = Path.Combine(relPathCurrentConfig, config.LicencePath);
                    if (File.Exists(SplashImagePath))
                    {
                        ApplicationSplashCode = "splash.png";
                        File.Copy(SplashImagePath, Path.Combine(sFilesFolder, ApplicationSplashCode), true);
                        ApplicationSplashCode = $"<EmbeddedResource Include=\"{ApplicationSplashCode}\" />";
                    }
                    else
                    {
                        actionLogAndCurrentMessage($"Splash Image not found: {config.SplashImagePath}");
                        return false;
                    }
                }

                string ApplicationIcon = null;
                if (!string.IsNullOrEmpty(config.ApplicationIconPath))
                {
                    string ApplicationIconPath = Path.Combine(relPathCurrentConfig, config.ApplicationIconPath);
                    if (File.Exists(ApplicationIconPath))
                    {
                        ApplicationIcon = "app.ico";
                        File.Copy(ApplicationIconPath, Path.Combine(sFilesFolder, ApplicationIcon), true);
                    }
                    else
                    {
                        actionLogAndCurrentMessage($"Application Icon not found: {config.ApplicationIconPath}");
                        return false;
                    }
                }


                //Add Application XAML and .CS
                //Copy from Embedded resources example of project file
                string ApplicationLanguageCodes = null;
                string csProjectSample = null;
                string csAppXamlSample = null;
                string csAppXamlCodeSample = null;
                string csUserScriptSample = null;

                using (var fileSource = Shared.GetEmbeddedResource("CompilerResources.DotNetCoreProject.SampleProject.csproj"))
                using (var reader = new StreamReader(fileSource))
                {
                    csProjectSample = await reader.ReadToEndAsync();
                }

                using (var fileSource = Shared.GetEmbeddedResource("CompilerResources.DotNetCoreProject.App.xaml"))
                using (var reader = new StreamReader(fileSource))
                {
                    csAppXamlSample = await reader.ReadToEndAsync();
                }


                using (var fileSource = Shared.GetEmbeddedResource("CompilerResources.DotNetCoreProject.App.xaml.cs"))
                using (var reader = new StreamReader(fileSource))
                {
                    csAppXamlCodeSample = await reader.ReadToEndAsync();
                }

                using (var fileSource = Shared.GetEmbeddedResource("CompilerResources.DotNetCoreProject.UserScripts.cs"))
                using (var reader = new StreamReader(fileSource))
                {
                    csUserScriptSample = await reader.ReadToEndAsync();
                }

                if (config.Languages?.Count > 0)
                {
                    foreach (var item in config.Languages)
                    {
                        if(File.Exists($"./{item}/NioTup.Lib.resources.dll"))
                        {
                            string sLanguageFile = $"NioTup.Lib.{item.Replace("-", "")}.resources.dll";
                            File.Copy($"./{item}/NioTup.Lib.resources.dll", Path.Combine(sFilesFolder, sLanguageFile), true);
                            ApplicationLanguageCodes += $"<EmbeddedResource Include=\"{sLanguageFile}\" />\n";
                        }
                    }                    
                }

                csProjectSample = csProjectSample.Replace("#AssemblyName", config.ApplicationAssemblyName);
                csProjectSample = csProjectSample.Replace("#AssemblyDescription", config.AssemblyDescription);
                csProjectSample = csProjectSample.Replace("#AssemblyCopyright", config.AssemblyCopyright);
                csProjectSample = csProjectSample.Replace("#AssemblyVersion", AssemblyVersion);
                csProjectSample = csProjectSample.Replace("#AssemblyFileVersion", AssemblyFileVersion);
                csProjectSample = csProjectSample.Replace("#AssemblyProduct", config.AssemblyProduct);
                csProjectSample = csProjectSample.Replace("#ApplicationIcon", ApplicationIcon);
                csProjectSample = csProjectSample.Replace("<!--#SplashReference-->", ApplicationSplashCode);
                csProjectSample = csProjectSample.Replace("<!--#LanguagesReference-->", ApplicationLanguageCodes);
                csProjectSample = csProjectSample.Replace("<!--#LicenceReference-->", LicenceCode);

                csUserScriptSample = csUserScriptSample.Replace("//#SetupEventHandleScripts", config.SetupEventHandleScripts);
                csUserScriptSample = csUserScriptSample.Replace("//#UserScripts", config.UserScripts);

                //Write .csproj file
                File.WriteAllText(projectFilePath, csProjectSample);

                //Write App.xaml file
                File.WriteAllText(Path.Combine(sFilesFolder, "App.xaml"), csAppXamlSample);
                
                //Write App.xaml.cs file
                File.WriteAllText(Path.Combine(sFilesFolder, "App.xaml.cs"), csAppXamlCodeSample);

                //Write UserScriptss file
                File.WriteAllText(Path.Combine(sFilesFolder, "UserScripts.cs"), csUserScriptSample);
                
                //Copy Lib
                File.Copy("./NioTup.Lib.dll", Path.Combine(sFilesFolder, "NioTup.Lib.dll"), true);
                File.Copy("./CommandLine.dll", Path.Combine(sFilesFolder, "CommandLine.dll"), true);

                //File.Copy("./pt-Br/NioTup.Lib.resources.dll", Path.Combine(sFilesFolder, "NioTup.Lib.ptbr.resources.dll"), true);

                List<FilesCheck> FilesChecker = new List<FilesCheck>();

                var configFullPath = new FileInfo(configPath).DirectoryName;

                Environment.CurrentDirectory = configFullPath;

                foreach (var files in config.Files)
                {
                    FilesCheck check = new FilesCheck();
                    FilesChecker.Add(check);

                    foreach (var dir in files.To)
                    {
                        check.Destinations.Add(dir);
                    }

                    foreach (var file in files.From)
                    {
                        var relPath = Path.GetRelativePath(configFullPath, file.FileName);
                        var pathFull = Path.GetFullPath(relPath);

                        if (pathFull.Contains("*"))
                        {
                            string SearchPattern = System.IO.Path.GetFileName(pathFull);
                            string SearchPath = System.IO.Path.GetDirectoryName(pathFull);
                            if (!Directory.Exists(SearchPath))
                            {
                                LogMessage($"\n*** Directory not found: {SearchPath} ***\n");
                                continue;
                            }

                            foreach (var fileInPath in Directory.GetFiles(SearchPath, SearchPattern, SearchOption.TopDirectoryOnly))
                            {
                                var fileName = new FileInfo(fileInPath).FullName;
                                if (check.Sources.Contains(fileName.ToLower().Trim()))
                                {
                                    LogMessage($"Duplicated File not found: {file.FileName}");
                                }
                                else
                                {
                                    check.Sources.Add(fileName);
                                }
                            }

                            if (!file.TopDirectoryOnly)
                            {
                                Action<string, List<string>> recursiveSearch = null;
                                recursiveSearch = (string searchDir, List<string> targetDirs) =>
                                {
                                    FilesCheck FileSubs = new FilesCheck();
                                    targetDirs.ForEach(x => FileSubs.Destinations.Add(x));

                                    foreach (var fileInPath in Directory.GetFiles(searchDir, SearchPattern, SearchOption.TopDirectoryOnly))
                                    {
                                        var fileName = new FileInfo(fileInPath).FullName;
                                        FileSubs.Sources.Add(fileName);
                                    }

                                    if (FileSubs.Sources.Count > 0 && FileSubs.Destinations.Count > 0)
                                    {
                                        FilesChecker.Add(FileSubs);
                                    }

                                    foreach (var dirInPath in Directory.GetDirectories(searchDir, SearchPattern))
                                    {
                                        List<string> targetDirsSub = new List<string>();
                                        var currentDirFullPath = new DirectoryInfo(dirInPath);
                                        FileSubs.Destinations.ForEach(x => targetDirsSub.Add(Path.Combine(x, currentDirFullPath.Name)));
                                        recursiveSearch(dirInPath, targetDirsSub);
                                    }
                                };
                                foreach (var dirInPath in Directory.GetDirectories(SearchPath))
                                {
                                    var directoriesToCreate = new List<string>();
                                    var currentDirFullPath = new DirectoryInfo(dirInPath);
                                    check.Destinations.ForEach(x => directoriesToCreate.Add(Path.Combine(x, currentDirFullPath.Name)));

                                    recursiveSearch(dirInPath, directoriesToCreate);
                                }
                            }
                        }
                        else
                        {
                            if (!File.Exists(file.FileName))
                            {
                                LogMessage($"File not found: {file.FileName}");
                            }
                            else
                            {
                                var fileName = new FileInfo(file.FileName).FullName;
                                if (check.Sources.Contains(fileName.ToLower().Trim()))
                                {
                                    LogMessage($"Duplicated File not found: {file.FileName}");
                                }
                                else
                                {
                                    check.Sources.Add(fileName);
                                }
                            }
                        }
                    }
                }

                LogMessage($"Total files to be Zipped {FilesChecker.Sum(x => x.Sources.Count)}");

                AppShared.ZipFiles(config, Path.Combine(sFilesFolder, "files.zip"), FilesChecker, setupData.Files, CurrentMessage);

                LogMessage($"Files Zipped");

                //Write setup.config.json
                File.WriteAllText(Path.Combine(sFilesFolder, "setup.config.json"), System.Text.Json.JsonSerializer.Serialize(setupData));

                string outPutPath = Path.GetFullPath(config.OutputPath);
                string outPutExeName = Path.Combine(outPutPath, config.ApplicationAssemblyName + ".exe");
                if (File.Exists(outPutExeName))
                {
                    File.Delete(outPutExeName);
                }
               
                Environment.CurrentDirectory = AppShared.defaultCurrentDirectory;

                var pathApp = Path.GetFullPath(sFilesFolder);

                //Project will build and run with current application allowing debug on visual studio
                if (Debug)
                {
                    LogMessage("Debug Compiling\n");
                    //Use dotnet to build
                    var piDebug = new ProcessStartInfo("dotnet")
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        Arguments = $"build {pathApp}",
                        CreateNoWindow = true
                    };
                    var procDebug = Process.Start(piDebug);
                    string textDebug = null;
                    string dllPath = null;
                    while ((textDebug = await procDebug.StandardOutput.ReadLineAsync()) != null)
                    {
                        if (textDebug != null)
                        {
                            if (textDebug.Contains(config.ApplicationAssemblyName + " -> "))
                            {
                                dllPath = textDebug.Substring((config.ApplicationAssemblyName + " -> ").Length + 2);
                            }
                        }
                        LogMessage(textDebug);
                    }
                    await procDebug.WaitForExitAsync();

                    var context = new System.Runtime.Loader.AssemblyLoadContext("test", true);
                    try
                    {
                        using (var str = File.Open(dllPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            Assembly asm = context.LoadFromStream(str);

                            var appType = asm.DefinedTypes.Where(x => x.Name == "App").FirstOrDefault();
                            var methodDebugRun = appType.GetMethod("DebugRun", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

                            //Run on UI Thread to avoid exception
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                methodDebugRun.Invoke(null, null);
                            });
                            context.Unload();
                        }
                    }
                    catch (Exception)
                    {
                        context?.Unload();
                        throw;
                    }
                    return procDebug.ExitCode == 0;
                }

                LogMessage("Compiling\n");

                //Use dotnet to Publish
                var pi = new ProcessStartInfo("dotnet")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,

                    Arguments = $"publish {pathApp} --output {outPutPath}",
                    CreateNoWindow = true
                };

                var proc = Process.Start(pi);

                string text = null;
                while ((text = await proc.StandardOutput.ReadLineAsync()) != null)
                {
                    LogMessage(text);
                }
                await proc.WaitForExitAsync();

                return proc.ExitCode == 0;
            }
            catch (Exception ex)
            {
                actionLogAndCurrentMessage(ex.Message + "\n" + ex.InnerException?.Message);
            }
            return false;
        }

        public void Dispose()
        {
            Environment.CurrentDirectory = AppShared.defaultCurrentDirectory;
            string sPath = Path.Combine(Path.GetTempPath(), "NioTupLib", PathID);
            if (Directory.Exists(sPath))
                Directory.Delete(sPath, true);
        }
    }
}
