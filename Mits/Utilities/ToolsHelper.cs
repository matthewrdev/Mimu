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
				new RepairImageReferencesTool(),
				new MigrateImagesTool(),
				new RenameImagesTool(),
                new DeduplicateImagesTools(),
            }.ToDictionary(t => t.Name, t => t);
		}
	}
}

