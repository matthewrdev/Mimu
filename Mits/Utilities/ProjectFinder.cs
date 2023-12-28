using System;
using System.Collections.Generic;
using Mits.Models;

namespace Mits.Utilities
{
    /// <summary>
    /// Searches
    /// </summary>
    public static class ProjectFinder
    {
        public static IReadOnlyList<string> excludedDirectories = new List<string>()
        {
            "bin",
            "obj",
        };

        private const string mauiProjectFileMarker = "<UseMaui>true</UseMaui>";
        private const string xamarinIOSFileMarker = "<Import Project=\"$(MSBuildExtensionsPath)\\Xamarin\\iOS\\Xamarin.iOS.CSharp.targets\" />";
        private const string xamarinAndroidFileMarker = "<Import Project=\"$(MSBuildExtensionsPath)\\Xamarin\\Android\\Xamarin.Android.CSharp.targets\" />";

        public const string ProjectFileExtension = "csproj";

        public static List<Project> FindAllProjects(string sourceDirectory)
        {
            List<Project> projects = new List<Project>();

            try
            {
                bool searchChildren = true;
                foreach (string filePath in Directory.GetFiles(sourceDirectory))
                {
                    var fileInfo = new FileInfo(filePath);

                    if (fileInfo.Extension == ProjectFileExtension)
                    {
                        searchChildren = false;
                        var projectKind = GetProjectFileKind(filePath);
                        projects.Add(new Project(fileInfo, projectKind));
                    }
                }

                if (searchChildren)
                {
                    foreach (string childDirectory in Directory.GetDirectories(sourceDirectory))
                    {
                        var directoryInfo = new DirectoryInfo(childDirectory);
                        if (excludedDirectories.Contains(directoryInfo.Name.ToLowerInvariant()))
                        {
                            continue;
                        }

                        var childResults = FindAllProjects(childDirectory);

                        if (childResults.Any())
                        {
                            projects.AddRange(childResults);
                        }
                    }
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            return projects;
        }

        public static ProjectKind GetProjectFileKind(string projectFilePath)
        {
            if (string.IsNullOrWhiteSpace(projectFilePath))
            {
                throw new ArgumentException($"'{nameof(projectFilePath)}' cannot be null or whitespace.", nameof(projectFilePath));
            }

            if (!File.Exists(projectFilePath))
            {
                throw new ArgumentException($"'{nameof(projectFilePath)}' must exist on disk.", nameof(projectFilePath));
            }

            var fileInfo = new FileInfo(projectFilePath);
            if (fileInfo.Extension != ProjectFileExtension)
            {
                throw new ArgumentException($"'{nameof(projectFilePath)}' is not a dotnet project file (the file extension must be '.{ProjectFileExtension}').", nameof(projectFilePath));
            }

            var contents = File.ReadAllText(fileInfo.FullName);

            if (contents.Contains(mauiProjectFileMarker, StringComparison.InvariantCultureIgnoreCase))
            {
                return ProjectKind.Maui;
            }
            else if (contents.Contains(xamarinIOSFileMarker, StringComparison.InvariantCultureIgnoreCase))
            {
                return ProjectKind.XamariniOS;
            }
            else if (contents.Contains(xamarinAndroidFileMarker, StringComparison.InvariantCultureIgnoreCase))
            {
                return ProjectKind.XamarinAndroid;
            }

            return ProjectKind.Other;
        }
    }
}

