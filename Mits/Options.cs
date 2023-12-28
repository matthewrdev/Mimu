using System;
using System.ComponentModel;
using Mits.Models;
using Newtonsoft.Json;

namespace Mits
{
	public static class Options
	{
        /// <summary>
        /// Renders the help text for the tool
        /// </summary>
        [Description("Displays the help text for Mits.")]
        public const string Help = "--help";

        /// <summary>
        /// Specifies the tool to run.
        /// </summary>
        [Description("Specifies the tool to run.")]
		public const string Tool = "--tool";

        /// <summary>
        /// Specifies the source directory for the tool
        /// </summary>
        [Description("Specifies the source folder for the given tool.")]
        public const string Source = "--source";

        /// <summary>
        /// Specifies the output directory or target for the tool
        /// </summary>
        [Description("Specifies the destination folder that the given tool should export to.")]
        public const string Destination = "--destination";

        /// <summary>
        /// Specifies the ruleset json file.
        /// </summary>
        [Description("Specifies the fully qualified path to the rule-set json file.")]
        public const string RuleSet = "--rule-set";

        /// <summary>
        /// Specifies the plain text file containing newline seperated image asset names to ignore.
        /// </summary>
        [Description("Specifies the fully qualified path for the plain text file containing newline seperated image asset names to ignore.")]
        public const string Excluded = "--excluded";

        /// <summary>
        /// Specifies that this run of MITS should only report the changes it would make and not apply them.
        /// </summary>
        [Description("Specifies that this run of MITS should only report the changes it would make and not apply them.")]
        public const string DryRun = "--dry-run";

        public static bool IsHelp(List<string> args)
        {
            return args.Contains(Help);
        }

        public static ToolConfiguration OptionsToToolConfiguration(List<string> args)
        {
            if (args.Contains(Help))
            {
                return null;
            }

            var tool = GetOption(Tool, args);
            var source = GetOption(Source, args);
            var destination = GetOption(Destination, args);
            var ruleSetFile = GetOption(RuleSet, args);
            var excluded = GetOption(Excluded, args);
            var isDryRun = args.Contains(DryRun);

            var ruleSet = LoadRuleSet(ruleSetFile);
            var excludedFiles = LoadExcludedFiles(excluded);

            return new ToolConfiguration(tool, source, destination, ruleSet, excludedFiles, isDryRun);
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

        private static ImageReferenceRules LoadRuleSet(string ruleSetFile)
        {
            if (string.IsNullOrWhiteSpace(ruleSetFile))
            {
                return ImageReferenceRules.Default;
            }

            if (!File.Exists(ruleSetFile))
            {
                return ImageReferenceRules.Default;
            }

            try
            {
                return JsonConvert.DeserializeObject<ImageReferenceRules>(File.ReadAllText(ruleSetFile));

            }
            catch
            {

            }

            return ImageReferenceRules.Default;
        }

        private static string GetOption(string optionName, List<string> args)
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

            var nextValueIndex = index + 1;
            if (nextValueIndex >= args.Count)
            {
                return string.Empty;
            }

            var nextValue = args[nextValueIndex];
            var isNextValueOption = nextValue.StartsWith("--");
            if (isNextValueOption)
            {
                return string.Empty;
            }

            return nextValue;
        }
    }
}

