using System;
using Mits.Models;

namespace Mits.Tools
{
	public class DetectMissingImagesTool : ITool
    {
        public string Name => "detect-missing-images";

        public string Help => $"Locates all MAUI projects within the given {Options.Source} folder path, scans for image references in XAML and C# files and then checks that these images exist.";

        public void Run(ToolConfiguration config)
        {
            throw new NotImplementedException();
        }
    }
}
