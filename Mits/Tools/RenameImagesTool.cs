using System;
using Mits.Models;

namespace Mits.Tools
{
	public class RenameImagesTool : ITool
	{
        public string Name => "rename";

        public string Help => $"Locates all MAUI projects within the given {Options.Source} folder path, collects their image assets and then renames them to be compliant with the MAUI image naming restrictions.";

        public void Run(ToolConfiguration config)
        {
            throw new NotImplementedException();
        }
    }
}

