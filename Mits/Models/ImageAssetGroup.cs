using System;
using Mits.Models;
using System.Collections.Generic;
using Mits.Utilities;

namespace Mits.Models
{
    public class ImageAssetGroup : IImageAsset
	{
        public ImageAssetGroup(string name, IReadOnlyList<ImageAsset> imageAssets, Project project)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            Name = name;

            TopLevelImage = ImageSizeHelper.GetTopLevelImage(imageAssets, project);
            ImageAssets = imageAssets ?? throw new ArgumentNullException(nameof(imageAssets));
            Project = project ?? throw new ArgumentNullException(nameof(project));
            Extension = Path.GetExtension(TopLevelImage.FilePath);
        }

		public string Name { get; }

		public IReadOnlyList<ImageAsset> ImageAssets { get; }

        public Project Project { get; }
        public ImageAsset TopLevelImage { get; }

        public string FilePath => TopLevelImage.FilePath;

        public string Extension { get; }
    }
}

