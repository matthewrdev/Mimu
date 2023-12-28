using System;
using Mits.Logging;
using Mits.Models;
using Mits.Utilities;

namespace Mits.Tools
{
	/// <summary>
	/// Moves the images from a legacy Xamarin.iOS or Xamarin.Android project into a specific MAUI project.
	/// </summary>
	public class MigrateImagesTool : ITool
    {
        private readonly ILogger log = Logger.Create();

        public string Name => "migrate";

        public string Help => $"Takes the Xamarin.iOS and Xamarin.Android projects within the given {Options.Source} path (this can be either a folder or a csproj), collects their image assets and then copies them into the 'Resources/Images' folder of any MAUI projects in the {Options.Destination} folder path or csproj.";

        public void Run(ToolConfiguration config)
        {
            if (!Validate(config))
            {
                return;
            }

            var sourceProjects = ProjectFinder.FindAllProjects(config.Source);
            log.Info("Discovered the following projects as the SOURCES for image migration:");

            foreach (var project in sourceProjects)
            {
                var isIgnored = project.ProjectKind == ProjectKind.Maui || project.ProjectKind == ProjectKind.Other;
                log.Info(" => " + project.ToString() + (isIgnored ? " [IGNORED]" : ""));
            }

            log.Info(Constants.LineBreak);

            var destinationProjects = ProjectFinder.FindAllProjects(config.Destination);

            log.Info("Discovered the following projects as the DESTINATIONS for image migration:");
            foreach (var project in destinationProjects)
            {
                var isIgnored = project.ProjectKind == ProjectKind.Maui || project.ProjectKind == ProjectKind.Other;
                log.Info(" => " + project.ToString() + (isIgnored ? " [IGNORED]" : ""));
            }

            var sourceImages = ImageAssetCollector.CollectImageAssets(sourceProjects);

            log.Info(Constants.LineBreak);

            log.Info("Discovered the following image assets for migration:");
            foreach (var image in sourceImages)
            {
                var isExcluded = config.Excluded.Contains(image.Name);
                log.Info(" => " + (isExcluded ? "[EXCLUDED]" : "") + image + " | CompatName=" + image.CompatName);

                foreach (var destination in destinationProjects.Where(d => d.ProjectKind == ProjectKind.Maui))
                {
                    var filePath = ImagePathHelper.GetFilePath(image, destination);
                    log.Info(" ===> Output: '" + filePath + "' in " + destination);
                }
            }

            if (config.DryRun)
            {
                return;
            }
        }

        private bool Validate(ToolConfiguration config)
        {
            var source = config.Source;
            var destination = config.Destination;

            if (string.IsNullOrWhiteSpace(source))
            {
                log.Error("No source provided");
                return false;
            }

            if (string.IsNullOrWhiteSpace(destination))
            {
                log.Error("No destination provided");
                return false;
            }

            var sourceFileExist = File.Exists(config.Source);
            var sourceDirectoryExists = Directory.Exists(config.Source);

            var hasValidSource = sourceFileExist || sourceDirectoryExists;

            if (!hasValidSource)
            {
                log.Error($"The provided source path '{config.Source}' does not exist.");
                return false;
            }

            if (sourceFileExist && Path.GetExtension(config.Source) != Constants.ProjectFileExtension)
            {
                log.Error($"The provided source file '{config.Source}' is not a valid project file. Expected the file extension '{Constants.ProjectFileExtension}'.");
                return false;
            }

            var destinationFileExist = File.Exists(config.Destination);
            var destinationDirectoryExists = Directory.Exists(config.Destination);

            var hasValidDestination = destinationFileExist || destinationDirectoryExists;
            if (!hasValidDestination)
            {
                log.Error($"The provided destination path '{config.Source}' does not exist.");
                return false;
            }

            if (destinationFileExist && Path.GetExtension(config.Destination) != Constants.ProjectFileExtension)
            {
                log.Error($"The provided destination file '{config.Destination}' is not a valid project file. Expected the file extension '{Constants.ProjectFileExtension}'.");
                return false;
            }

            return true;
        }
    }
}