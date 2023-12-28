using System;
using Mits.Models;

namespace Mits.Utilities
{
	public static class ImageAssetCollector
	{
		public static IReadOnlyList<IImageAsset> CollectImageAssets(Project project)
		{
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            switch (project.ProjectKind)
            {
                case ProjectKind.Maui:
                    return CollectImageAssets_Maui(project);
                case ProjectKind.XamariniOS:
                    return CollectImageAssets_XamariniOS(project);
                case ProjectKind.XamarinAndroid:
                    return CollectImageAssets_XamarinAndroid(project);
                case ProjectKind.Other:
                default:
                    return Array.Empty<IImageAsset>();
            }
        }

        private static IReadOnlyList<IImageAsset> CollectImageAssets_XamarinAndroid(Project project)
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
                ScanAndCollectImages(project, drawableFolders, groups);
            }

            if (mipmapFolders.Any())
            {
                ScanAndCollectImages(project, mipmapFolders, groups);
            }

            foreach (var g in groups)
            {
                if (g.Value.Count == 1)
                {
                    imageAssets.Add(g.Value.First());
                }
                else
                {
                    imageAssets.Add(new ImageAssetGroup(g.Key, g.Value, project));
                }
            }

            return imageAssets;
        }

        private static void ScanAndCollectImages(Project project, List<string> drawableFolders, Dictionary<string, List<ImageAsset>> groups)
        {
            foreach (var folder in drawableFolders)
            {
                var images = FileFinder.FindAllFiles(folder, Constants.ImageFileExtensions);

                foreach (var image in images)
                {
                    var imageName = Path.GetFileNameWithoutExtension(image.Name);

                    if (!groups.ContainsKey(imageName))
                    {
                        groups[imageName] = new List<ImageAsset>();
                    }

                    groups[imageName].Add(new ImageAsset(imageName, image.FullName, image.Extension, project));
                }
            }
        }

        private static IReadOnlyList<IImageAsset> CollectImageAssets_XamariniOS(Project project)
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

                    if (!groups.ContainsKey(imageName))
                    {
                        groups[imageName] = new List<ImageAsset>();
                    }

                    groups[imageName].Add(new ImageAsset(imageName, image.FullName, image.Extension, project));
                }

                foreach (var g in groups)
                {
                    if (g.Value.Count == 1)
                    {
                        imageAssets.Add(g.Value.First());
                    }
                    else
                    {
                        imageAssets.Add(new ImageAssetGroup(g.Key, g.Value, project));
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

            var assets = images.Select(i => new ImageAsset(Path.GetFileNameWithoutExtension(i.Name), i.FullName, i.Extension, project)).ToList();

            return new ImageAssetGroup(imageName,
                                       assets,
                                       project);


        }

        private static IReadOnlyList<IImageAsset> CollectImageAssets_Maui(Project project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            var resourcesFolder = Path.Combine(project.Folder, "Resources");

            if (Directory.Exists(resourcesFolder))
            {
                return Array.Empty<IImageAsset>();
            }

            var images = FileFinder.FindAllFiles(resourcesFolder, Constants.ImageFileExtensions);


            List<IImageAsset> imageAssets = new List<IImageAsset>();
            Dictionary<string, List<ImageAsset>> groups = new Dictionary<string, List<ImageAsset>>();

            foreach (var image in images)
            {
                var imageName = Path.GetFileNameWithoutExtension(image.Name);

                if (imageName.Contains("@"))
                {
                    imageName = imageName.Split("@").First();
                }

                if (!groups.ContainsKey(imageName))
                {
                    groups[imageName] = new List<ImageAsset>();
                }

                groups[imageName].Add(new ImageAsset(imageName, image.FullName, image.Extension, project));
            }

            foreach (var g in groups)
            {
                if (g.Value.Count == 1)
                {
                    imageAssets.Add(g.Value.First());
                }
                else
                {
                    imageAssets.Add(new ImageAssetGroup(g.Key, g.Value, project));
                }
            }

            return imageAssets;
        }
    }
}

