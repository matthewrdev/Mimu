using System;
using Mits.Logging;
using Mits.Models;
using Mits.Utilities;

namespace Mits.Tools
{
	public class RenameImagesTool : ITool
    {
        private readonly ILogger log = Logger.Create();

        public string Name => "rename";

        public string Help => $"Locates all MAUI projects within the given {Options.Source} folder path, collects their image assets and then renames them to be compliant with the MAUI image naming restrictions.";

        public void Run(ToolConfiguration config)
        {
            if (!Validate(config, out var targetProject))
            {
                return;
            }

            var sourceImages = ImageAssetFinder.FindImageAssets(targetProject);

            log.Info(Constants.LineBreak);

            log.Info("Discovered the following image assets for renaming:");
            foreach (var image in sourceImages.Where(i => string.Compare( i.Name, i.CompatName, StringComparison.Ordinal) != 0))
            {
                var isExcluded = config.Excluded.Contains(image.Name);
                log.Info(" => " + (isExcluded ? "[EXCLUDED]" : "") + image + " | CompatName=" + image.CompatName + image.Extension);
            }

            if (config.DryRun)
            {
                return;
            }


            log.Info(Constants.LineBreak);
            log.Info("Renaming image assets...");
            foreach (var image in sourceImages.Where(i => string.Compare(i.Name, i.CompatName, StringComparison.Ordinal) != 0))
            {
                var isExcluded = config.Excluded.Contains(image.Name);
                if (isExcluded)
                {
                    log.Info(" Skipping " + image);
                    continue;
                }

                var destinationFilePath = ImagePathHelper.GetFilePath(image, targetProject);
                var exists = File.Exists(destinationFilePath);

                if (!exists)
                {
                    continue;
                }

                try
                {
                    if (!config.OverWrite)
                    {
                        log.Warning($"Skipping {image.FilePath} as its destination file, {destinationFilePath}, already exists.");
                        continue;
                    }

                    File.Copy(image.FilePath, destinationFilePath, overwrite: true);
                    log.Info(" => Renamed " + image.FilePath + " to " + destinationFilePath);
                }
                finally
                {
                    if (!config.KeepExistingImages)
                    {
                        log.Info(" => Deleting source image " + image.FilePath);
                        File.Delete(image.FilePath);
                    }

                }
            }
        }

        private bool Validate(ToolConfiguration config, out Project targetProject)
        {
            targetProject = ProjectFinder.FindAllProjects(config.Source).FirstOrDefault();

            if (targetProject is null)
            {
                log.Error($"Unable to locate the target project file '{config.Left}'.");
                return false;
            }

            if (targetProject.ProjectKind != ProjectKind.Maui)
            {
                log.Error($"The target project file '{config.Left}' is not a valid .NET MAUI project.");
                return false;
            }

            return true;
        }
    }
}

