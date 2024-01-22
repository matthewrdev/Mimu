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
                                 bool dryRun,
                                 bool overWrite,
                                 ImageNumericBehaviour numericSufixBehaviour,
                                 ImageNumericBehaviour numericPrefixBehaviour,
                                 PreserveBehaviour preserveBehaviour,
                                 bool keepExistingImages)
        {
            ToolName = toolName;
            Source = source;
            Destination = destination;
            Ruleset = ruleset ?? ImageReferenceConfiguration.Default;
            Excluded = ImmutableHashSet.Create((excluded ?? Array.Empty<string>()).ToArray());
            DryRun = dryRun;
            OverWrite = overWrite;
            NumericSuffixBehaviour = numericSufixBehaviour;
            NumericPrefixBehaviour = numericPrefixBehaviour;
            PreserveBehaviour = preserveBehaviour;
            KeepExistingImages = keepExistingImages;
        }

        public string ToolName { get; }

        public string Source { get; }
        public string Left => Source;

        public string Destination { get; }
        public string Right => Destination;

        public ImageReferenceConfiguration Ruleset { get; }

		public ImmutableHashSet<string> Excluded { get; }

		public bool DryRun { get; }

        public bool OverWrite { get; }

        public ImageNumericBehaviour NumericSuffixBehaviour { get; }

        public ImageNumericBehaviour NumericPrefixBehaviour { get; }
        public PreserveBehaviour PreserveBehaviour { get; }
        public bool KeepExistingImages { get; }
    }
}

