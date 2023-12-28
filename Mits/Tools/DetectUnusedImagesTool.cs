using System;
using Mits.Models;

namespace Mits.Tools
{
	public class DetectUnusedImagesTool : ITool
	{
        public string Name => "detect-unused-images";

        public string Help => $"Locates all MAUI projects within the given {Options.Source} folder path, collects all images owned by that project and then checks that each image is referenced somewhere within the XAML and C# files.";

        public void Run(ToolConfiguration config)
        {
            throw new NotImplementedException();
        }
    }
}

