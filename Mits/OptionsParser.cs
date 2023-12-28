using System;
using Microsoft.CodeAnalysis;
using Mits.Models;
using Mits.Tools;
using Newtonsoft.Json;

namespace Mits
{
	public static class OptionsParser
    {
        public static bool IsHelp(List<string> args)
        {
            return args == null || args.Count == 0 || args.Contains(Options.Help);
        }

        public static ToolConfiguration OptionsToToolConfiguration(List<string> args)
        {
            if (args.Contains(Options.Help))
            {
                return null;
            }

            var tool = GetOptionValue(Options.Tool, args);
            var source = GetOptionValue(Options.Source, args);
            var destination = GetOptionValue(Options.Destination, args);
            var ruleSetFile = GetOptionValue(Options.RuleSet, args);
            var excluded = GetOptionValue(Options.Excluded, args);
            var isDryRun = args.Contains(Options.DryRun);
            var overwrite = args.Contains(Options.Overwrite);
            var suffixBehaviour = GetOptionValue(Options.NumericSuffixBehaviour, args);
            var prefixBehaviour = GetOptionValue(Options.NumericPrefixBehaviour, args);

            var ruleSet = LoadRuleSet(ruleSetFile);
            var excludedFiles = LoadExcludedFiles(excluded);

            return new ToolConfiguration(tool,
                                         source,
                                         destination,
                                         ruleSet,
                                         excludedFiles,
                                         isDryRun,
                                         overwrite,
                                         suffixBehaviour == "to-word" ? ImageNumericBehaviour.ToWord : ImageNumericBehaviour.Ammend,
                                         prefixBehaviour == "to-word" ? ImageNumericBehaviour.ToWord : ImageNumericBehaviour.Ammend);
        }

        private static IReadOnlyList<string> LoadExcludedFiles(string excluded)
        {
            if (string.IsNullOrWhiteSpace(excluded))
            {
                return Array.Empty<string>();
            }

            if (!File.Exists(excluded))
            {
                return Array.Empty<string>();
            }

            return File.ReadAllLines(excluded);
        }

        private static ImageReferenceConfiguration LoadRuleSet(string ruleSetFile)
        {
            if (string.IsNullOrWhiteSpace(ruleSetFile))
            {
                return ImageReferenceConfiguration.Default;
            }

            if (!File.Exists(ruleSetFile))
            {
                return ImageReferenceConfiguration.Default;
            }

            try
            {
                return JsonConvert.DeserializeObject<ImageReferenceConfiguration>(File.ReadAllText(ruleSetFile));

            }
            catch
            {
            }

            return ImageReferenceConfiguration.Default;
        }

        private static string GetOptionValue(string optionName, List<string> args)
        {
            if (string.IsNullOrEmpty(optionName))
            {
                throw new ArgumentException($"'{nameof(optionName)}' cannot be null or empty.", nameof(optionName));
            }

            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var index = args.IndexOf(optionName);
            if (index < 0)
            {
                return string.Empty;
            }

            var nextArgsIndex = index + 1;
            if (nextArgsIndex >= args.Count)
            {
                return string.Empty;
            }

            var nextArgument = args[nextArgsIndex];
            var isNextArgumentOption = nextArgument.StartsWith("--");
            if (isNextArgumentOption)
            {
                return string.Empty;
            }

            return nextArgument;
        }
    }
}

