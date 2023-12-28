using System;
using Mits.Models;

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
            FilePath = filePath;
            Extension = extension;
            Project = project ?? throw new ArgumentNullException(nameof(project));
        }

		public string Name { get; }

		public string FilePath { get; }
        public string Extension { get; }
        public Project Project { get; }
    }
}

