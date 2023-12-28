using System;
using System.Collections.Immutable;
using Mimu.Models;

namespace Mimu.Utilities
{
    /// <summary>
    /// Searches a given project for all known image references.
    /// </summary>
    public static class ImageReferenceFinder
    {
        public const string XamlFileExtension = ".xaml";
        public const string CSharpFileExtension = ".cs";

        public static readonly IReadOnlyList<string> imageFileExtensions = new List<string>()
        {
            ".png",
            ".jpeg",
            ".jpg",
            ".gif",
        };

        public static bool IsReferenceFindable(Project project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            return project.ProjectKind == ProjectKind.Maui;
        }

        public static IReadOnlyDictionary<string, IReadOnlyList<ImageReference>> FindReferences(IReadOnlyList<Project> projects, bool searchCSharp, bool searchXaml)
        {
            if (projects is null)
            {
                throw new ArgumentNullException(nameof(projects));
            }

            if (searchCSharp == false && searchXaml == false)
            {
                return ImmutableDictionary<string, IReadOnlyList<ImageReference>>.Empty;
            }

            Dictionary<string, List<ImageReference>> references = new Dictionary<string, List<ImageReference>>();

            foreach (var project in projects)
            {
                var result = FindReferences(project, searchCSharp, searchXaml);
            }

            return references.ToDictionary(kp => kp.Key, kp => (IReadOnlyList<ImageReference>)kp.Value);
        }

        public static IReadOnlyDictionary<string, IReadOnlyList<ImageReference>> FindReferences(Project project, bool searchCSharp, bool searchXaml)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (!IsReferenceFindable(project))
            {
                return ImmutableDictionary<string, IReadOnlyList<ImageReference>>.Empty;
            }

            if (searchCSharp == false && searchXaml == false)
            {
                return ImmutableDictionary<string, IReadOnlyList<ImageReference>>.Empty;
            }

            var projectFolder = project.Folder;
            var extensions = GetFileExtensionsForSearch(searchCSharp, searchXaml);

            var files = FileFinder.FindAllFiles(projectFolder, extensions);
            if (!files.Any())
            {
                return ImmutableDictionary<string, IReadOnlyList<ImageReference>>.Empty;
            }

            Dictionary<string, IReadOnlyList<ImageReference>> references = new Dictionary<string, IReadOnlyList<ImageReference>>();
            foreach (var file in files)
            {

                IReadOnlyList<ImageReference> fileReferences = FindReferences(file);

                if (fileReferences != null && fileReferences.Any())
                {
                    references[file.FullName] = fileReferences;
                }
            }

            return references;
        }

        public static IReadOnlyList<ImageReference> FindReferences(FileInfo fileInfo)
        {
            if (fileInfo is null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            switch (fileInfo.Extension)
            {
                case XamlFileExtension:
                    return FindXamlReferences(fileInfo);
                case CSharpFileExtension:
                    return FindCSharpReferences(fileInfo);
                default:
                    return Array.Empty<ImageReference>();
            }
        }


        public static IReadOnlyList<ImageReference> FindXamlReferences(FileInfo fileInfo)
        {
            if (fileInfo is null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }


        }

        public static IReadOnlyList<ImageReference> FindCSharpReferences(FileInfo fileInfo)
        {
            if (fileInfo is null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }
        }

        private enum ParserState
        {
            /// <summary>
            /// The scanner is currently looking for " literals
            /// </summary>
            SearchingForStrings,

            /// <summary>
            /// The scanner is currently within a string literal and is building a string buffer.
            /// </summary>
            BuildingStringBuffer,
        }

        public static IReadOnlyList<ImageReference> FindStringReferences(string contents)
        {
            var parserState = ParserState.SearchingForStrings;

            var spanStart = 0;
            var currentValueBuffer = "";



            for (var i = 0; i < contents.Length; i++)
            {
                var currentChar = contents[i];

                if (currentChar == '"')
                {
                    switch (parserState)
                    {
                        case ParserState.SearchingForStrings:
                            break;
                        case ParserState.BuildingStringBuffer:
                            break;
                    }
                }
                else
                {
                    switch (parserState)
                    {
                        case ParserState.SearchingForStrings:
                            break;
                        case ParserState.BuildingStringBuffer:
                            break;
                    }
                }
            }
        }

        public static bool TryParseToImageReference(string value, out string imageReference)
        {
            imageReference = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
        }

        public static IReadOnlyList<string> GetFileExtensionsForSearch(bool searchCSharp, bool searchXaml)
        {
            var extensions = new List<string>();

            if (searchCSharp)
            {
                extensions.Add(CSharpFileExtension);
            }

            if (searchXaml)
            {
                extensions.Add(XamlFileExtension);
            }

            return extensions;
        }
    }
}

