using System;
using System.Drawing;
using FileRenamer;
using Mits.Models;
using Mits.Utilities;

namespace Mits.Models
{
	public class ImageAsset : IImageAsset
	{
        public ImageAsset(string name,
                         string filePath,
                         string extension,
                         Project project)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or whitespace.", nameof(filePath));
            }

            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException($"'{nameof(extension)}' cannot be null or empty.", nameof(extension));
            }

            Name = name;
            CompatName = ImageNameCompatibilityHelper.ConvertToCompatibleName(name, out _);
            FilePath = filePath;
            Extension = extension;
            Project = project ?? throw new ArgumentNullException(nameof(project));
            Size = ImageSizeHelper.GetImageSize(FilePath);
        }

		public string Name { get; }

		public string FilePath { get; }
        public string Extension { get; }
        public Project Project { get; }
        public Size Size { get; }

        public string CompatName { get; }

        public override string ToString()
        {
            return Name + Extension + " in " + Project.Name + $" ({Size.Width}w {Size.Height}h)";
        }
    }
}

