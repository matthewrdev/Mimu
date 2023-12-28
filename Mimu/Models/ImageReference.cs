using System;
using Microsoft.CodeAnalysis.Text;

namespace Mimu.Models
{
	public class ImageReference
	{
        public ImageReference(string text,
                              TextSpan span,
                              string filePath,
                              string project)
        {
            Text = text;
            Span = span;
            FilePath = filePath;
            Project = project;
        }

		public string Text { get; }

		public TextSpan Span { get; }

		public string FilePath { get; }

		public string Project { get; }
	}
}

