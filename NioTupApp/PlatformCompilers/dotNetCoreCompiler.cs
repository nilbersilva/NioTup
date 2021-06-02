using Microsoft.CodeAnalysis;
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
                string CompilationMainFolder = Path.Combine("./", "NioTupLib", PathID);
                Directory.CreateDirectory(CompilationMainFolder);

                string sFilesFolder = Path.Combine(CompilationMainFolder, "Compile");
                sFilesFolder = Path.GetFullPath(sFilesFolder);
                Directory.CreateDirectory(sFilesFolder);

                string projectFilePath = Path.Combine(sFilesFolder, $"{config.ApplicationAssemblyName}.csproj");



            

                //Add Application XAML and .CS
                string applicationSource = appCS.Replace("#APP_NAME#", config.ApplicationAssemblyName);
             

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


               
                string csprojFileText = projectString(!string.IsNullOrWhiteSpace(config.SplashImagePath),
                !string.IsNullOrWhiteSpace(config.ApplicationIconPath), !string.IsNullOrWhiteSpace(config.LicencePath),
                config.AssemblyDescription, config.AssemblyCopyright, AssemblyVersion, AssemblyFileVersion,config.AssemblyProduct);
                if (config.Platform == Platform.X86)
                {
                    csprojFileText = csprojFileText.Replace("win-x64", "win-x86");
                }
                //Create WPF Project
                File.WriteAllText(projectFilePath, csprojFileText);

                File.WriteAllText(Path.Combine(sFilesFolder, "App.xaml.cs"), applicationSource);
                File.WriteAllText(Path.Combine(sFilesFolder, "App.xaml"), appXaml.Replace("#APP_NAME#", config.ApplicationAssemblyName));

                //Add Solution
                string solutionName = Path.Combine(sFilesFolder, $"{config.ApplicationAssemblyName}.sln");
                File.WriteAllText(solutionName, solutionSource.Replace("#APP_NAME#", config.ApplicationAssemblyName));

                var pathApp = Path.GetFullPath(sFilesFolder);

                //Copy Lib
                File.Copy("./NioTup.Lib.dll", Path.Combine(sFilesFolder, "NioTup.Lib.dll"), true);
                File.Copy("./CommandLine.dll", Path.Combine(sFilesFolder, "CommandLine.dll"), true);

                File.Copy("./pt-Br/NioTup.Lib.resources.dll", Path.Combine(sFilesFolder, "NioTup.Lib.ptbr.resources.dll"), true);

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


                SetupInfo setupData = new SetupInfo
                {
                    Files = new List<SetupDataFile>(),
                };

                //Copy Default Properties to Setup Config Data
                Shared.CopyPropertiesTo(config, setupData);

                AppShared.ZipFiles(config, Path.Combine(sFilesFolder, "files.zip"), FilesChecker, setupData.Files, CurrentMessage);

                LogMessage($"Files Zipped");



                if (!string.IsNullOrEmpty(config.LicencePath))
                {
                    if (File.Exists(config.LicencePath))
                    {
                        File.Delete(Path.Combine(sFilesFolder, "licence.zip"));
                        using (var fileReader = File.Open(config.LicencePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var destFile = File.Open(Path.Combine(sFilesFolder, "licence.zip"), FileMode.OpenOrCreate, FileAccess.Write))
                        using (var zipLience = new GZipStream(destFile, CompressionLevel.Optimal))
                        {
                            await fileReader.CopyToAsync(zipLience);
                            setupData.HasLicence = true;
                        }
                    }
                    else
                    {
                        actionLogAndCurrentMessage($"Licence file not found: {config.SplashImagePath}");
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(config.SplashImagePath))
                {
                    if (File.Exists(config.SplashImagePath))
                    {
                        File.Copy(config.SplashImagePath, Path.Combine(sFilesFolder, "splash.png"), true);
                    }
                    else
                    {
                        actionLogAndCurrentMessage($"Splash Image not found: {config.SplashImagePath}");
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(config.ApplicationIconPath))
                {
                    if (File.Exists(config.ApplicationIconPath))
                    {
                        File.Copy(config.ApplicationIconPath, Path.Combine(sFilesFolder, "app.ico"), true);
                    }
                    else
                    {
                        actionLogAndCurrentMessage($"Application Icon not found: {config.ApplicationIconPath}");
                        return false;
                    }
                }

                //Write setup.config.json
                File.WriteAllText(Path.Combine(sFilesFolder, "setup.config.json"), System.Text.Json.JsonSerializer.Serialize(setupData));


                string outPutPath = Path.GetFullPath(config.OutputPath);
                string outPutExeName = Path.Combine(outPutPath, config.ApplicationAssemblyName + ".exe");
                if (File.Exists(outPutExeName))
                {
                    File.Delete(outPutExeName);
                }

                Environment.CurrentDirectory = AppShared.defaultCurrentDirectory;


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

        public static string projectString(bool includeSplash, bool applicationIcon, bool licenceFile, string assemblyDescription,string  assemblyCopyright, string AssemblyVersion, string FileVersion, string assemblyProduct)
        {
            return @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net5.0-windows</TargetFramework>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <PublishReadyToRun>true</PublishReadyToRun>
    <PublishSingleFile>true</PublishSingleFile>
    <SelfContained>false</SelfContained>
    <DebugType>embedded</DebugType>
    <OutputType>Exe</OutputType>
    <UseWPF>true</UseWPF>
    <UseWindowsForms>true</UseWindowsForms>
    <Description>" + assemblyDescription  + @"</Description>
    <Copyright>" + assemblyCopyright + @"</Copyright>
    <AssemblyVersion>" + AssemblyVersion  + @"</AssemblyVersion>
    <Version>" + FileVersion  + @"</Version>
    <Product>" + assemblyProduct + @"</Product>
" + (applicationIcon ? "\n<ApplicationIcon>app.ico</ApplicationIcon>" : "") +
    @"
  </PropertyGroup>
  
  <PropertyGroup>
    <RollForward>Minor</RollForward>
  </PropertyGroup>  

  <ItemGroup>
    <Reference Include=""NioTup.Lib"">
      <HintPath>.\NioTup.Lib.dll</HintPath>  
     </Reference>  
    <Reference Include=""CommandLine"">
      <HintPath>.\CommandLine.dll</HintPath>  
      </Reference>  
    </ItemGroup>
" + (includeSplash ? @"<ItemGroup>
    <Resource Include=""splash.png""  />
  </ItemGroup>
" : "") +
  @"<ItemGroup>
    <EmbeddedResource Include =""setup.config.json"" />
    <EmbeddedResource Include = ""files.zip"" />
    <EmbeddedResource Include = ""NioTup.Lib.ptbr.resources.dll"" />
" + (licenceFile ? @"<EmbeddedResource Include = ""licence.zip"" />" : "") + @"
  </ItemGroup>
</Project>
";
        }

        static string appCS = @"using System;
using System.Windows;
using System.IO;
using System.Reflection;
using NioTup.Lib;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]


namespace #APP_NAME# {  
    
    public partial class App : Application
    {        
        static App()
        {
            try
            {
                Shared.MainApplicationAssembly = typeof(App).Assembly;
                SplashScreen splashScreen = new SplashScreen(typeof(App).Assembly, ""splash.png"");
                splashScreen.Show(true);
            }
            catch(Exception)
            {}
        }

        public App()
        {
             AppDomain.CurrentDomain.AssemblyResolve += currentDomain_AssemblyResolve;
        }

        private Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if(args.Name.Contains(""NioTup.Lib"") && args.Name.Contains(""resources""))
            {
                if (System.Globalization.CultureInfo.CurrentUICulture.Name != ""en-US"" && System.Globalization.CultureInfo.CurrentUICulture.Name != ""en-GB"")
                {
                    var resource = typeof(App).Assembly.GetManifestResourceStream(typeof(App).Assembly.GetName().Name + "".NioTup.Lib."" + System.Globalization.CultureInfo.CurrentUICulture.Name.Replace(""-"","""").ToLower() + "".resources.dll"");

                    if(resource!=null) 
                    {
                         using(var memStream = new MemoryStream())
                         {
                            resource.CopyTo(memStream); 
                            return Assembly.Load(memStream.ToArray());
                         }
                    }
                }
            }
            return null;
        }
        
        public static void DebugRun() 
        {
            Shared.AppStart();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Shared.AppStart();
        }
    }
}";

        static string appXaml = @"<Application x:Class=""#APP_NAME#.App""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             Startup=""Application_Startup"">
    <Application.Resources>         
    </Application.Resources>
</Application>
";

        static string solutionSource = @"Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 16
VisualStudioVersion = 16.0.31313.79
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""#APP_NAME#"", ""#APP_NAME#.csproj"", ""{AEB79FD9-7ACE-44F5-9E2D-416C367253AB}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{5DAF87A1-F3B2-463F-8C59-92091CA51DFA}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{5DAF87A1-F3B2-463F-8C59-92091CA51DFA}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{5DAF87A1-F3B2-463F-8C59-92091CA51DFA}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{5DAF87A1-F3B2-463F-8C59-92091CA51DFA}.Release|Any CPU.Build.0 = Release|Any CPU
		{AEB79FD9-7ACE-44F5-9E2D-416C367253AB}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{AEB79FD9-7ACE-44F5-9E2D-416C367253AB}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{AEB79FD9-7ACE-44F5-9E2D-416C367253AB}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{AEB79FD9-7ACE-44F5-9E2D-416C367253AB}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {6C888B75-246F-4E16-BE37-51E91080263E}
	EndGlobalSection
EndGlobal
";
    }
}
