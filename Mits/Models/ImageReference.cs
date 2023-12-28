using System;
using FileRenamer;
using Microsoft.CodeAnalysis.Text;

namespace Mits.Models
{
	public class ImageReference
	{
        public ImageReference(string text,
                              TextSpan span,
                              string filePath,
                              string project)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"'{nameof(text)}' cannot be null or empty.", nameof(text));
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            if (string.IsNullOrEmpty(project))
            {
                throw new ArgumentException($"'{nameof(project)}' cannot be null or empty.", nameof(project));
            }

            OriginalImageReference = text;
            Extension = Path.GetExtension(text);
            Span = span;
            FilePath = filePath;
            Project = project;
            var convertedName = ImageNameCompatibilityHelper.ConvertToCompatibleName(OriginalImageReference, out var didConvert);
            CompatImageReference = didConvert ? convertedName + Extension : text;

            HasConversion = didConvert;
        }

		public string OriginalImageReference { get; }
        public string Extension { get; }
        public string CompatImageReference { get; }

        public bool HasConversion { get; }

		public TextSpan Span { get; }

		public string FilePath { get; }

		public string Project { get; }

        public override string ToString()
        {
            return OriginalImageReference + " at " + Span + $" in '{FilePath}' in '{Project}'";
        }
    }
}

