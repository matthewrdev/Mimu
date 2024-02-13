using System;
using System.Text.RegularExpressions;
using Mits.Logging;
using Mits.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Mits.Utilities
{
	public static class ImageAssetFinder
    {
        private static readonly ILogger log = Logger.Create();

        public static IReadOnlyList<IImageAsset> FindImageAssets(IReadOnlyList<Project> projects)
        {
            if (projects is null)
            {
                throw new ArgumentNullException(nameof(projects));
            }

            if (!projects.Any())
            {
                return Array.Empty<IImageAsset>();
            }

            Dictionary<string, List<ImageAsset>> groups = new Dictionary<string, List<ImageAsset>>();

            foreach (var project in projects)
            {
                var images = FindImageAssets(project);
                if (!images.Any())
                {
                    continue;
                }

                foreach (var image in images)
                {
                    var imageName = Path.GetFileNameWithoutExtension(image.Name);

                    if (!groups.ContainsKey(imageName))
                    {
                        groups[imageName] = new List<ImageAsset>();
                    }

                    if (image is ImageAsset imageAsset)
                    {
                        groups[imageName].Add(imageAsset);
                    }
                    else if (image is ImageAssetGroup imageAssetGroup)
                    {
                        groups[imageName].AddRange(imageAssetGroup.ImageAssets);
                    }
                }
            }

            List<IImageAsset> imageAssets = new List<IImageAsset>();


            foreach (var g in groups)
            {
                if (g.Value.Count == 1)
                {
                    imageAssets.Add(g.Value.First());
                }
                else
                {
                    imageAssets.Add(new ImageAssetGroup(g.Key, g.Value));
                }
            }

            return imageAssets;

        }

        public static IReadOnlyList<IImageAsset> FindImageAssets(Project project)
		{
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            switch (project.ProjectKind)
            {
                case ProjectKind.Maui:
                    return FindImageAssets_Maui(project);
                case ProjectKind.XamariniOS:
                    return FindImageAssets_XamariniOS(project);
                case ProjectKind.XamarinAndroid:
                    return FindImageAssets_XamarinAndroid(project);
                case ProjectKind.Other:
                default:
                    return Array.Empty<IImageAsset>();
            }
        }

        private static IReadOnlyList<IImageAsset> FindImageAssets_XamarinAndroid(Project project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            var resourcesFolder = Path.Combine(project.Folder, "Resources");

            if (!Directory.Exists(resourcesFolder))
            {
                return Array.Empty<IImageAsset>();
            }

            var directories = Directory.GetDirectories(resourcesFolder);

            var drawableFolders = new List<string>();
            var mipmapFolders = new List<string>();

            foreach (var dir in directories)
            {
                var info = new DirectoryInfo(dir);

                if (info.Name.StartsWith(Constants.DrawableFolderPrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    drawableFolders.Add(info.FullName);
                }

                if (info.Name.StartsWith(Constants.MipMapFolderPrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    mipmapFolders.Add(info.FullName);
                }
            }

            List<IImageAsset> imageAssets = new List<IImageAsset>();
            Dictionary<string, List<ImageAsset>> groups = new Dictionary<string, List<ImageAsset>>();

            if (drawableFolders.Any())
            {
                FindImagesInAndroidFolders(project, drawableFolders, groups);
            }

            if (mipmapFolders.Any())
            {
                FindImagesInAndroidFolders(project, mipmapFolders, groups);
            }

            foreach (var g in groups)
            {
                if (g.Value.Count == 1)
                {
                    imageAssets.Add(g.Value.First());
                }
                else
                {
                    imageAssets.Add(new ImageAssetGroup(g.Key, g.Value));
                }
            }

            return imageAssets;
        }

        private static void FindImagesInAndroidFolders(Project project, List<string> drawableFolders, Dictionary<string, List<ImageAsset>> groups)
        {
            foreach (var folder in drawableFolders)
            {
                var images = FileFinder.FindAllFiles(folder, Constants.ImageFileExtensions);

                foreach (var image in images)
                {
                    var imageName = Path.GetFileNameWithoutExtension(image.Name);

                    try
                    {
                        var asset = new ImageAsset(imageName, image.FullName, image.Extension, project);

                        if (!groups.ContainsKey(imageName))
                        {
                            groups[imageName] = new List<ImageAsset>();
                        }

                        groups[imageName].Add(asset);
                    }
                    catch (Exception ex)
                    {
                        log.Error($"An error occured while including the image '{image.FullName}'. This image will be skipped.");
                        log.Error(ex.GetType() + ": " + ex.Message);
                    }
                }
            }
        }

        private static IReadOnlyList<IImageAsset> FindImageAssets_XamariniOS(Project project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            var directories = Directory.GetDirectories(project.Folder);

            var assetSets = new List<string>();

            foreach (var directory in directories)
            {
                var info = new DirectoryInfo(directory);

                if (info.Name.EndsWith(Constants.AssetSetFolderExtension))
                {
                    assetSets.Add(directory);
                }
            }

            List<IImageAsset> imageAssets = new List<IImageAsset>();

            var resourcesFolder = Path.Combine(project.Folder, "Resources");

            if (Directory.Exists(resourcesFolder))
            {
                Dictionary<string, List<ImageAsset>> groups = new Dictionary<string, List<ImageAsset>>();
                var images = FileFinder.FindAllFiles(resourcesFolder, Constants.ImageFileExtensions);

                foreach (var image in images)
                {
                    var imageName = Path.GetFileNameWithoutExtension(image.Name);

                    if (imageName.Contains("@"))
                    {
                        imageName = imageName.Split("@").First();
                    }

                    try
                    {
                        var asset = new ImageAsset(imageName, image.FullName, image.Extension, project);

                        if (!groups.ContainsKey(imageName))
                        {
                            groups[imageName] = new List<ImageAsset>();
                        }

                        groups[imageName].Add(asset);
                    }
                    catch (Exception ex)
                    {
                        log.Error($"An error occured while including the image '{image.FullName}'. This image will be skipped.");
                        log.Error(ex.GetType() + ": " + ex.Message);
                    }
                }

                foreach (var g in groups)
                {
                    if (g.Value.Count == 1)
                    {
                        imageAssets.Add(g.Value.First());
                    }
                    else
                    {
                        imageAssets.Add(new ImageAssetGroup(g.Key, g.Value));
                    }
                }
            }

            if (assetSets.Any())
            {
                foreach (var assetSet in assetSets)
                {
                    var innerFolders  = Directory.GetDirectories(assetSet);

                    foreach (var folder in innerFolders)
                    {
                        var info = new DirectoryInfo(folder);

                        if (info.Name.EndsWith(Constants.AppIconSetFolderExtension)
                            || info.Name.EndsWith(Constants.ImageSetFolderExtension))
                        {
                            var imageSet = BuildImageSetFromFolder(info, project);
                            if (imageSet != null)
                            {
                                imageAssets.Add(imageSet);
                            }
                        }
                    }
                }
            }

            return imageAssets;
        }

        private static IImageAsset BuildImageSetFromFolder(DirectoryInfo info, Project project)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            var imageName = info.Name;

            if (imageName.EndsWith(Constants.AppIconSetFolderExtension))
            {
                imageName = imageName.Substring(0, imageName.Length - Constants.AppIconSetFolderExtension.Length);
            }
            else if (imageName.EndsWith(Constants.ImageSetFolderExtension))
            {
                imageName = imageName.Substring(0, imageName.Length - Constants.ImageSetFolderExtension.Length);
            }

            var images = FileFinder.FindAllFiles(info.FullName, Constants.ImageFileExtensions);

            if (images.Count == 0)
            {
                return null;
            }

            var assets = images.Select(i =>
            {
                try
                {
                    return new ImageAsset(i.Name, i.FullName, i.Extension, project);
                }
                catch (Exception ex)
                {
                    log.Error($"An error occured while including the image '{i.FullName}'. This image will be skipped.");
                    log.Error(ex.GetType() + ": " + ex.Message);
                }
                return null;
            }).Where(a => a != null)
              .ToList();

            return new ImageAssetGroup(imageName,
                                       assets);


        }

        private static IReadOnlyList<IImageAsset> FindImageAssets_Maui(Project project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            var resourcesFolder = Path.Combine(project.Folder, "Resources");

            if (!Directory.Exists(resourcesFolder))
            {
                return Array.Empty<IImageAsset>();
            }

            var images = FileFinder.FindAllFiles(resourcesFolder, Constants.ImageFileExtensions);

            List<IImageAsset> imageAssets = new List<IImageAsset>();
            Dictionary<string, List<ImageAsset>> groups = new Dictionary<string, List<ImageAsset>>();

            foreach (var image in images)
            {
                var imageName = Path.GetFileNameWithoutExtension(image.Name);


                try
                {
                    var asset = new ImageAsset(imageName, image.FullName, image.Extension, project);

                    if (!groups.ContainsKey(imageName))
                    {
                        groups[imageName] = new List<ImageAsset>();
                    }

                    groups[imageName].Add(asset);
                }
                catch (Exception ex)
                {
                    log.Error($"An error occured while including the image '{image.FullName}'. This image will be skipped.");
                    log.Error(ex.GetType() + ": " + ex.Message);
                }
            }

            foreach (var g in groups)
            {
                if (g.Value.Count == 1)
                {
                    imageAssets.Add(g.Value.First());
                }
                else
                {
                    imageAssets.Add(new ImageAssetGroup(g.Key, g.Value));
                }
            }

            return imageAssets;
        }
    }
}

