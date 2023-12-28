using System;
using System.Drawing;
using Mits.Models;
using SkiaSharp;

namespace Mits.Utilities
{
	public static class ImageSizeHelper
	{
		public static Size GetImageSize(string filePath)
		{
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or whitespace.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' must exist on disk.", nameof(filePath));
            }

            using (var stream = File.OpenRead(filePath))
            {
                using (var bitmap = SKBitmap.Decode(stream))
                {
                    return new Size(bitmap.Width, bitmap.Height);
                }
            }
        }

        internal static ImageAsset GetTopLevelImage(IReadOnlyList<ImageAsset> imageAssets, Project project)
        {
            if (imageAssets is null)
            {
                throw new ArgumentNullException(nameof(imageAssets));
            }

            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            // Assumes images are same aspect and uses a width-dominent size preference rather than the specific sizing per platform.
            // Good enough for v1 I guess!s
            return imageAssets.OrderByDescending(i => GetImageSize(i.FilePath).Width).FirstOrDefault();
        }
    }
}

