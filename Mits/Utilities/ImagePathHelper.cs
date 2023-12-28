using System;
using Mits.Models;

namespace Mits.Utilities
{
	public static class ImagePathHelper
	{
		public static string GetFilePath(IImageAsset imageAsset, Project project)
		{
            if (imageAsset is null)
            {
                throw new ArgumentNullException(nameof(imageAsset));
            }

            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            return Path.Combine(project.Folder, Constants.ResourcesFolder, Constants.ImagesFolder, imageAsset.CompatName + imageAsset.Extension);
        }
	}
}

