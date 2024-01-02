using System;
using Microsoft.VisualBasic;
using Mits.Logging;
using Mits.Models;
using Mits.Utilities;

namespace Mits.Tools
{
	public class RepairImageReferencesTool : ITool
    {
        private readonly ILogger log = Logger.Create();

        public string Name => "repair";

        public string Help => $"Locates all MAUI projects within the given {Options.Source} folder path, scans for image references within XAML and C# files and then converts those image references into a MAUI compliant image name.";

        public void Run(ToolConfiguration config)
        {
            if (!Validate(config))
            {
                return;
            }

            var projects = ProjectFinder.FindAllProjects(config.Source);

            var mauiProjects = projects.Where(p => p.ProjectKind == ProjectKind.Maui).ToList();
            if (!mauiProjects.Any())
            {
                log.Error("No MAUI projects were found to fix image references.");
                return;
            }

            var sourceProjects = ProjectFinder.FindAllProjects(config.Source);
            log.Info("Discovered the following projects as targets for image reference fixing:");

            foreach (var project in mauiProjects)
            {
                log.Info(" => " + project.ToString());
            }

            log.Info(Constants.LineBreak);

            Dictionary<Project, IReadOnlyDictionary<string, IReadOnlyList<ImageReference>>> projectIndexedReferences = new Dictionary<Project, IReadOnlyDictionary<string, IReadOnlyList<ImageReference>>>();

            foreach (var project in mauiProjects)
            {
                log.Info("Scanning " + project + " for image references...");

                var imageReferences = ImageReferenceFinder.FindReferences(project, searchCSharp: true, searchXaml: true, config.Ruleset);
                projectIndexedReferences[project] = imageReferences;

                var totalReferences = projectIndexedReferences.Sum(r => r.Value.Count);
                var fixableReferences = projectIndexedReferences.Sum(r => r.Value.Sum(v => v.Value.Count(v2 => v2.HasConversion)));

                log.Info($"Done! Found {totalReferences} image references in {imageReferences.Count} .xaml and .cs files with {imageReferences.Count} distinct images. There are {fixableReferences} references requiring fixing.");

                log.Info(Constants.LineBreak);
            }

            foreach (var projectReferenceSet in projectIndexedReferences)
            {
                log.Info($"Repairing image references for project: {projectReferenceSet.Key}");

                foreach (var referenceSet in projectReferenceSet.Value)
                {
                    var filePath = referenceSet.Key;
                    var references = referenceSet.Value;

                    log.Info($" => Repairing image references for file: {filePath}");

                    if (!File.Exists(filePath))
                    {
                        log.Warning($" => Skipping {filePath} as it does not exist on disk");
                        continue;
                    }

                    try
                    {
                        var contents = File.ReadAllText(filePath);
                        var hashStart = MD5Helper.FromString(contents);

                        var invertedReferences = references.OrderByDescending(r => r.Span.Start).ToList();

                        int changeCount = 0;
                        foreach (var reference in invertedReferences)
                        {
                            if (reference.HasConversion)
                            {
                                changeCount++;
                                log.Info($" ====> Replacing '{reference.OriginalImageReference}' with '{reference.CompatImageReference}' at '{reference.Span}'");
                                contents = contents.Remove(reference.Span.Start, reference.Span.Length).Insert(reference.Span.Start, reference.CompatImageReference);
                            }
                        }

                        var hashEnd = MD5Helper.FromString(contents);
                        if (!config.DryRun)
                        {
                            if (hashStart == hashEnd)
                            {
                                log.Info($" ===> No changes were applied to '{filePath}'.");
                            }
                            else
                            {
                                log.Info($" ===> Applied {changeCount} changes '{filePath}'. Staring file hash '{hashStart}', ending file hash '{hashEnd}'");
                                File.WriteAllText(filePath, contents);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Exception(ex);
                    }
                }

                log.Info(Constants.LineBreak);
                log.Info(Constants.LineBreak);
            }
        }


        private bool Validate(ToolConfiguration config)
        {
            var source = config.Source;
            
            if (string.IsNullOrWhiteSpace(source))
            {
                log.Error("No source provided");
                return false;
            }

            var sourceFileExist = File.Exists(config.Source);
            var sourceDirectoryExists = Directory.Exists(config.Source);

            var hasValidSource = sourceFileExist || sourceDirectoryExists;

            if (!hasValidSource)
            {
                log.Error($"The provided source path '{config.Source}' does not exist.");
                return false;
            }

            if (sourceFileExist && Path.GetExtension(config.Source) != Constants.ProjectFileExtension)
            {
                log.Error($"The provided source file '{config.Source}' is not a valid project file. Expected the file extension '{Constants.ProjectFileExtension}'.");
                return false;
            }

            return true;
        }
    }
}