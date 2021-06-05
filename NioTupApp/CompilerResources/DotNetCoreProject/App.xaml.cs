using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using NioTup.Lib;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]

namespace NioTupApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            try
            {
                Shared.MainApplicationAssembly = typeof(App).Assembly;
                SplashScreen splashScreen = new SplashScreen(typeof(App).Assembly, "splash.png");
                splashScreen.Show(true);
            }
            catch (Exception)
            { }
        }


        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += currentDomain_AssemblyResolve;
        }

        private Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("NioTup.Lib") && args.Name.Contains("resources"))
            {
                if (System.Globalization.CultureInfo.CurrentUICulture.Name != "en-US" && System.Globalization.CultureInfo.CurrentUICulture.Name != "en-GB")
                {
                    string sResourceName = typeof(App).Assembly.GetName().Name + ".NioTup.Lib." + System.Globalization.CultureInfo.CurrentUICulture.Name.Replace("-", "").ToLower() + ".resources.dll";
                    sResourceName = typeof(App).Assembly.GetManifestResourceNames().Where(x => x.ToLower() == sResourceName.ToLower()).FirstOrDefault();
                    var resource = typeof(App).Assembly.GetManifestResourceStream(sResourceName);

                    if (resource != null)
                    {
                        using (var memStream = new MemoryStream())
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
            NioTupUserScripts.NioTupUserScriptsInit();
            Shared.AppStart();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            NioTupUserScripts.NioTupUserScriptsInit();
            Shared.AppStart();
        }
    }
}
