using System;
using Mits.Models;
using System.Collections.Generic;
using Mits.Utilities;
using System.Drawing;
using FileRenamer;

namespace Mits.Models
{
    public class ImageAssetGroup : IImageAsset
	{
        public ImageAssetGroup(string name,
                               IReadOnlyList<ImageAsset> imageAssets)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            Name = name;
            CompatName = ImageNameCompatibilityHelper.ConvertToCompatibleName(name, out _);
            TopLevelImage = ImageSizeHelper.GetTopLevelImage(imageAssets);
            ImageAssets = imageAssets ?? throw new ArgumentNullException(nameof(imageAssets));
            Extension = Path.GetExtension(TopLevelImage.FilePath);
            Projects = ImageAssets.Select(i => i.Project).Distinct().ToList();
        }

		public string Name { get; }

		public IReadOnlyList<ImageAsset> ImageAssets { get; }

        public IReadOnlyList<Project> Projects { get; }

        public Project Project => TopLevelImage.Project;
        public ImageAsset TopLevelImage { get; }

        public string FilePath => TopLevelImage.FilePath;

        public string Extension { get; }

        public Size Size => TopLevelImage.Size;

        public string CompatName { get; }

        public override string ToString()
        {
            return Name + Extension + " group in " + Project.Name + $"({ImageAssets.Count} values)" + $" ({Size.Width}w {Size.Height}h)";
        }
    }
}

