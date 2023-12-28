using System;
using System.Collections.Generic;

namespace Mits.Utilities
{
    /// <summary>
    /// Searches
    /// </summary>
    public static class FileFinder
    {
        public static IReadOnlyList<string> excludedDirectories = new List<string>()
        {
            "bin",
            "obj",
        };

        public static List<FileInfo> FindAllFiles(string sourceDirectory,
                                                  IReadOnlyList<string> targetFileExtensions)
        {
            List<FileInfo> matchedFiles = new List<FileInfo>();

            try
            {
                foreach (string f in Directory.GetFiles(sourceDirectory))
                {
                    var fileInfo = new FileInfo(f);

                    if (targetFileExtensions.Contains(fileInfo.Extension))
                    {
                        matchedFiles.Add(fileInfo);
                    }
                }

                foreach (string childDirectory in Directory.GetDirectories(sourceDirectory))
                {
                    var directoryInfo = new DirectoryInfo(childDirectory);
                    if (excludedDirectories.Contains(directoryInfo.Name.ToLowerInvariant()))
                    {
                        continue;
                    }

                    var childResults = FindAllFiles(childDirectory, targetFileExtensions);

                    if (childResults.Any())
                    {
                        matchedFiles.AddRange(childResults);
                    }
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            return matchedFiles;
        }
    }
}

