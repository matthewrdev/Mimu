using System;
using Mits.Logging;
using Mits.Models;
using Mits.Utilities;

namespace Mits.Tools
{
	public class DeduplicateImagesTools : ITool
    {
        private readonly ILogger log = Logger.Create();

        public string Name => "deduplicate";

        public string Help => $"For the given {Options.Left} and {Options.Right} projects, checks for duplicate images and deletes them in the {Options.Right} project.";

        public void Run(ToolConfiguration config)
        {
            if (!Validate(config, out var leftProject, out var rightProject))
            {
                return;
            }

            var leftProjectImages = ImageAssetFinder.FindImageAssets(leftProject);
            log.Info($"Found {leftProjectImages.Count} images in {leftProject.FilePath}");

            var rightProjectImages = ImageAssetFinder.FindImageAssets(rightProject);
            log.Info($"Found {rightProjectImages.Count} images in {rightProject.FilePath}");

            var leftValues = new HashSet<string>(leftProjectImages.Select(i => i.Name).ToList());

            var intersection = rightProjectImages.Where(im => leftValues.Contains(im.Name)).ToList();

            log.Info($"Found {intersection.Count} images that exist in both projects.");

            foreach (var image in intersection)
            {
                if (config.DryRun)
                {
                    log.Info($"The iamge asset '{image.FilePath}' would be deleted.");
                }
                else
                {
                    log.Info($"Deleting iamge asset '{image.FilePath}'.");

                    File.Delete(image.FilePath);
                }
            }
        }

        private bool Validate(ToolConfiguration config, out Project leftProject, out Project rightProject)
        {
            leftProject = ProjectFinder.FindAllProjects(config.Left).FirstOrDefault();
            rightProject = ProjectFinder.FindAllProjects(config.Right).FirstOrDefault();

            if (leftProject is null)
            {
                log.Error($"Unable to locate the left project file '{config.Left}'.");
                return false;
            }

            if (rightProject is null)
            {
                log.Error($"Unable to locate the right project file '{config.Right}'.");
                return false;
            }

            if (leftProject.ProjectKind != ProjectKind.Maui)
            {
                log.Error($"The left project file '{config.Left}' is not a valid .NET MAUI project.");
                return false;
            }

            if (rightProject.ProjectKind != ProjectKind.Maui)
            {
                log.Error($"The right project file '{config.Right}' is not a valid .NET MAUI project.");
                return false;
            }

            return true;
        }
    }
}

