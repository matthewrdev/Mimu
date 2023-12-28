using System;
using Mits.Models;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mits.Models
{
	public class ToolConfiguration
	{
        public ToolConfiguration(string toolName,
                                 string source,
                                 string destination,
                                 ImageReferenceConfiguration ruleset,
                                 IReadOnlyList<string> excluded,
                                 bool dryRun)
        {
            ToolName = toolName;
            Source = source;
            Destination = destination;
            Ruleset = ruleset ?? ImageReferenceConfiguration.Default;
            Excluded = ImmutableHashSet.Create((excluded ?? Array.Empty<string>()).ToArray());
            DryRun = dryRun;
        }

        public string ToolName { get; }

        public string Source { get; }

		public string Destination { get; }

		public ImageReferenceConfiguration Ruleset { get; }

		public ImmutableHashSet<string> Excluded { get; }

		public bool DryRun { get; }
	}
}

