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
        /// Specifies the tool to run.
        /// </summary>
        [Description("Specifies that the tool should overwrite any destination files and not skip them.")]
        public const string Overwrite = "--overwrite";

        /// <summary>
        /// Specifies the source directory for the tool
        /// </summary>
        [Description("Specifies the source folder or project file for the given tool.")]
        public const string Source = "--source";

        [Description("Specifies the left folder or project file for the given tool. This flag is an alias for the " + Source + " flag.")]
        public const string Left = "--left";

        /// <summary>
        /// Specifies the output directory or target for the tool
        /// </summary>
        [Description("Specifies the destination folder or project file that the given tool should export to.")]
        public const string Destination = "--destination";

        [Description("Specifies the right folder or project file for the given tool. This flag is an alias for the " + Destination + " flag.")]
        public const string Right = "--right";

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

        /// <summary>
        ///
        /// </summary>
        [Description("When an image file name ends with a number value, specifies the behaviour to repair it.\n\n'append': Appends '_n' to the end of the image name (Default).\n'to-word': Converts the number to a word representation.")]
        public const string NumericSuffixBehaviour = "--numeric-suffix-behaviour";

        /// <summary>
        ///
        /// </summary>
        [Description("When an image file name starts with a number value, specifies the behaviour to repair it.\n\n'append': Prepends 'n_' to the start of the image name (Default).\n'to-word': Converts the number to a word representation.")]
        public const string NumericPrefixBehaviour = "--numeric-prefix-behaviour";
    }
}