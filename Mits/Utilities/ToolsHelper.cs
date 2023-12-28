using System;
using Mits.Tools;

namespace Mits.Utilities
{
	public static class ToolsHelper
	{
		public static IReadOnlyDictionary<string, ITool> BuildTools()
		{
			return new List<ITool>()
			{
				new FixImageReferencesTool(),
				new MigrateImagesTool(),
				new RenameImagesTool(),
			}.ToDictionary(t => t.Name, t => t);
		}
	}
}

