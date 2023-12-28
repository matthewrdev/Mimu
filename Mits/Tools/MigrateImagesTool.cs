using System;
using Mits.Models;

namespace Mits.Tools
{
	/// <summary>
	/// Moves the images from a legacy Xamarin.iOS or Xamarin.Android project into a specific MAUI project.
	/// </summary>
	public class MigrateImagesTool : ITool
    {
        public string Name => "migrate";

        public string Help => $"Locates all Xamarin.iOS and Xamarin.Android projects within the given {Options.Source} folder path, collects their image assets and then copies them into the 'Resources/Images' folder of any MAUI projects in the {Options.Destination} folder path.";

        public void Run(ToolConfiguration config)
        {
            throw new NotImplementedException();
        }
    }
}