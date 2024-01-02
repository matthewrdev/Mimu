using System;
using System.Collections.Generic;
using System.IO;
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

        public static bool IsProjectFile(string filePath, out string reason)
        {
            reason = "";
            if (string.IsNullOrWhiteSpace(filePath))
            {
                reason = "No file path was provided";
                return false;
            }

            if (!File.Exists(filePath))
            {
                reason = $"The file at '{filePath}' does not exist.";
                return false;
            }

            if (Path.GetExtension(filePath) != Constants.ProjectFileExtension)
            {
                reason = $"The file at '{filePath}' is not a csproj file..";
                return false;
            }

            return true;
        }

        public static List<Project> FindAllProjects(string sourcePath)
        {
            List<Project> projects = new List<Project>();

            if (File.Exists(sourcePath))
            {
                if (Path.GetExtension(sourcePath) == Constants.ProjectFileExtension)
                {
                    var projectKind = GetProjectFileKind(sourcePath);
                    projects.Add(new Project(new FileInfo(sourcePath), projectKind));
                }

                return projects;
            }

            try
            {
                bool searchChildren = true;
                foreach (string filePath in Directory.GetFiles(sourcePath))
                {
                    var fileInfo = new FileInfo(filePath);

                    if (fileInfo.Extension == Constants.ProjectFileExtension)
                    {
                        searchChildren = false;
                        var projectKind = GetProjectFileKind(filePath);
                        projects.Add(new Project(fileInfo, projectKind));
                    }
                }

                if (searchChildren)
                {
                    foreach (string childDirectory in Directory.GetDirectories(sourcePath))
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
            if (fileInfo.Extension != Constants.ProjectFileExtension)
            {
                throw new ArgumentException($"'{nameof(projectFilePath)}' is not a dotnet project file (the file extension must be '{Constants.ProjectFileExtension}').", nameof(projectFilePath));
            }

            var contents = File.ReadAllText(fileInfo.FullName);

            if (contents.Contains(Constants.MauiProjectFileMarker, StringComparison.InvariantCultureIgnoreCase))
            {
                return ProjectKind.Maui;
            }
            else if (contents.Contains(Constants.XamarinIOSFileMarker, StringComparison.InvariantCultureIgnoreCase))
            {
                return ProjectKind.XamariniOS;
            }
            else if (contents.Contains(Constants.XamarinAndroidFileMarker, StringComparison.InvariantCultureIgnoreCase))
            {
                return ProjectKind.XamarinAndroid;
            }

            return ProjectKind.Other;
        }
    }
}

