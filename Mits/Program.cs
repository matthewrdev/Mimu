// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using FileRenamer;
using Mits;
using Mits.Logging;
using Mits.Models;
using Mits.Utilities;

Logger.Instance.Factory = new ConsoleLoggerFactory();

var log = Logger.Create();

var tools = ToolsHelper.BuildTools();

if (OptionsParser.IsHelp(args.ToList()))
{
    HelpRenderer.RenderHelp(tools.Values.ToList());
    return;
}

var config = OptionsParser.OptionsToToolConfiguration(args.ToList());

if (string.IsNullOrWhiteSpace(config.ToolName))
{
    Console.WriteLine("No tool was provided. Exiting.");
    return;
}

if (!tools.TryGetValue(config.ToolName, out var tool))
{
    return;
}

tool.Run(config);