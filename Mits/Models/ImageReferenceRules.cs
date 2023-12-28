using System;
using Mits.Models;
namespace Mits.Models
{
	public class ImageReferenceRules
	{
        public ImageReferenceRules(bool ignoreUrls, bool ignoreFormattedStrings)
        {
            IgnoreUrls = ignoreUrls;
            IgnoreFormattedStrings = ignoreFormattedStrings;
        }

		public bool IgnoreUrls { get; }

		public bool IgnoreFormattedStrings { get; }

		public static ImageReferenceRules Default { get; } = new ImageReferenceRules(ignoreUrls:true, ignoreFormattedStrings:true);
    }
}

