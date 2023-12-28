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
        [Description("Specifies the source folder or project file for the given tool.")]
        public const string Source = "--source";

        /// <summary>
        /// Specifies the output directory or target for the tool
        /// </summary>
        [Description("Specifies the destination folder or project file that the given tool should export to.")]
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
    }
}

