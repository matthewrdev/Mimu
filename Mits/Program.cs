// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using FileRenamer;
using Mits;
using Mits.Models;
using Mits.Utilities;

var tools = ToolsHelper.BuildTools();

var configuration = Options.OptionsToToolConfiguration(args.ToList());

var projects = ProjectFinder.FindAllProjects("/Users/matthewrobbins/Development/git/TestApp");

foreach (var project in projects)
{
    var iamges = ImageAssetCollector.CollectImageAssets(project);
}

var folderPath = "";

var images = Directory.GetFiles(folderPath, "*.png");

Dictionary<string, string> nameReplacements = new Dictionary<string, string>();

foreach (var image in images)
{
    var fileInfo = new FileInfo(image);

    if (ImageNameCompatibilityHelper.IsIncompatibleImageName(fileInfo.Name, out var validationError))
    {
        var newName = ImageNameCompatibilityHelper.ConvertToCompatibleName(fileInfo.Name, out _);

        newName += fileInfo.Extension;

        var newFilePath = Path.Combine(fileInfo.Directory.FullName, newName);
        nameReplacements[fileInfo.Name] = newName;

        if (File.Exists(newFilePath))
        {
            File.Delete(newFilePath);
        }

        Console.WriteLine($"Renaming {fileInfo.Name} to {newName}");

        File.Copy(fileInfo.FullName, newFilePath);

        File.Delete(fileInfo.FullName);

        Console.WriteLine("Converted " + fileInfo.Name);
    }
}

var files = FileFinder.FindAllFiles("path goes here", new List<string>() { ".cs", ".xaml" });

foreach (var file in files)
{
    var fileContent = File.ReadAllText(file.FullName);

    Console.WriteLine($" -> Trying to update image references in {file.FullName}");

    var didUpdate = false;
    foreach (var replacement in nameReplacements)
    {
        var oldValue = $"\"{replacement.Key}\"";
        var newValue = $"\"{replacement.Value}\"";

        if (!fileContent.Contains(oldValue, StringComparison.InvariantCultureIgnoreCase))
        {
            continue;
        }

        didUpdate = true;
        fileContent = fileContent.Replace(oldValue, newValue, StringComparison.InvariantCultureIgnoreCase);
    }

    if (!didUpdate)
    {
        continue;
    }

    Console.WriteLine($" -> Replaced image references in {file.FullName}");

    File.WriteAllText(file.FullName, fileContent);
}

Console.WriteLine("Done!");