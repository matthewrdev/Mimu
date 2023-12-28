using System;
using Mimu.Models;

namespace Mimu.Tools
{
	/// <summary>
	/// Moves the images from a legacy Xamarin.iOS or Xamarin.Android project into a specific MAUI project.
	/// </summary>
	public class MigrateImagesTool : ITool
	{
		public MigrateImagesTool()
		{
		}

		public string Name => "migrate";

        public void Run(IReadOnlyList<Project> projects)
        {
			// Find 
        }
    }
}

