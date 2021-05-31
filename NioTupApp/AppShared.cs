using NioTup.Lib.Models;
using NioTupApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NioTupApp
{
    public static class AppShared
    {
        public static string defaultCurrentDirectory { get; }

        static AppShared()
        {
            defaultCurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        public static void ZipFiles(SetupConfig config, string destinationFile, List<FilesCheck> FilesChecker, List<SetupDataFile> SetupFiles, Action<string> CurrentMessage)
        {
            int fileCount = 0;
            int totalFileCount = FilesChecker.Sum(x => x.Sources.Count);
            using (var archive = ZipFile.Open(destinationFile, ZipArchiveMode.Create))
            {
                int folderCount = 0;
                foreach (var item in FilesChecker)
                {
                    folderCount++;
                    SetupDataFile setupFile = new SetupDataFile();
                    SetupFiles.Add(setupFile);

                    setupFile.Destination = string.Join(';', item.Destinations);

                    foreach (var file in item.Sources)
                    {
                        fileCount++;
                        archive.CreateEntryFromFile(file, $"{folderCount}/{Path.GetFileName(file)}", config.CompressionLevel);
                        CurrentMessage?.Invoke($"Compressing {fileCount} of {totalFileCount} files");
                    }
                }
            }
        }
    }
}
