using System;
using Mits.Models;
using System.Collections.Generic;

namespace Mits.Models
{
	public class ToolConfiguration
	{
        public ToolConfiguration(string toolName,
                                 string source,
                                 string destination,
                                 ImageReferenceRules ruleset,
                                 IReadOnlyList<string> excluded,
                                 bool dryRun)
        {
            ToolName = toolName;
            Source = source;
            Destination = destination;
            Ruleset = ruleset ?? ImageReferenceRules.Default;
            Excluded = excluded ?? Array.Empty<string>();
            DryRun = dryRun;
        }

        public string ToolName { get; }
        public string Source { get; }

		public string Destination { get; }

		public ImageReferenceRules Ruleset { get; }

		public IReadOnlyList<string> Excluded { get; }

		public bool DryRun { get; }
	}
}

