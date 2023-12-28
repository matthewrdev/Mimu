using System;
using Mits.Models;

namespace Mits.Tools
{
	public class RenameImagesTool : ITool
	{
		public RenameImagesTool()
		{
		}

        public string Name => "rename";

        public void Run(IReadOnlyList<Project> projects)
        {
            // TODO: For MAUI projects, find the images and rename them using the compat name helper.
        }
    }
}

