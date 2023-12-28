using System;
using Mits.Models;

namespace Mits.Models
{
	public class ImageReferenceConfiguration
	{
        public ImageReferenceConfiguration(bool ignoreUrls,
                                           bool ignoreFormattedStrings)
        {
            IgnoreUrls = ignoreUrls;
            IgnoreFormattedStrings = ignoreFormattedStrings;
        }

		public bool IgnoreUrls { get; }

		public bool IgnoreFormattedStrings { get; }

		public static ImageReferenceConfiguration Default { get; } = new ImageReferenceConfiguration(ignoreUrls:true, ignoreFormattedStrings:true);
    }
}

