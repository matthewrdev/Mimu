using FileRenamer;
using Mits;
using Mits.Logging;
using Mits.Utilities;

Logger.Instance.Factory = new ConsoleLoggerFactory();

var tools = ToolsHelper.BuildTools();

if (OptionsParser.IsHelp(args.ToList()))
{
    HelpRenderer.RenderHelp(tools.Values.ToList());
    return;
}

var config = OptionsParser.OptionsToToolConfiguration(args.ToList());

ImageNameCompatibilityHelper.PrefixBehaviour = config.NumericPrefixBehaviour;
ImageNameCompatibilityHelper.SuffixBehaviour = config.NumericSuffixBehaviour;

if (string.IsNullOrWhiteSpace(config.ToolName))
{
    Console.WriteLine("No tool was provided. Exiting.");
    return;
}

if (!tools.TryGetValue(config.ToolName, out var tool))
{
    Console.WriteLine($"The specified tool '{config.ToolName}' does not exist.");
    return;
}

tool.Run(config);