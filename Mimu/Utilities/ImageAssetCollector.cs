using System;
using Mimu.Models;

namespace Mimu.Utilities
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

            // Search drawable
            // Search mipmap

            return Array.Empty<IImageAsset>();
        }

        private static IReadOnlyList<IImageAsset> CollectImageAssets_XamariniOS(Project project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            // Get all images under Resources
            // Get all image sets.

            return Array.Empty<IImageAsset>();
        }

        private static IReadOnlyList<IImageAsset> CollectImageAssets_Maui(Project project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            // Get all images under the Resources/ path.

            return Array.Empty<IImageAsset>();
        }
    }
}

