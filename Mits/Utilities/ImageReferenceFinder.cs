using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;
using Mits.Models;

namespace Mits.Utilities
{
    /// <summary>
    /// Searches a given project for all known image references.
    /// </summary>
    public static class ImageReferenceFinder
    {
        private const char EscapeCharacter = '\\';
        private const char StringLiteralCharacter = '"';
        private const char StringFormatStartCharacter = '{';
        private const char StringFormatEndCharacter = '}';

        public static bool IsReferenceFindable(Project project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            return project.ProjectKind == ProjectKind.Maui;
        }

        public static IReadOnlyDictionary<string, IReadOnlyList<ImageReference>> FindReferences(IReadOnlyList<Project> projects, bool searchCSharp, bool searchXaml, ImageReferenceConfiguration rules = null)
        {
            if (projects is null)
            {
                throw new ArgumentNullException(nameof(projects));
            }

            if (searchCSharp == false && searchXaml == false)
            {
                return ImmutableDictionary<string, IReadOnlyList<ImageReference>>.Empty;
            }

            rules ??= ImageReferenceConfiguration.Default;

            Dictionary<string, List<ImageReference>> references = new Dictionary<string, List<ImageReference>>();

            foreach (var project in projects)
            {
                var result = FindReferences(project, searchCSharp, searchXaml, rules);
            }

            return references.ToDictionary(kp => kp.Key, kp => (IReadOnlyList<ImageReference>)kp.Value);
        }

        public static IReadOnlyDictionary<string, IReadOnlyList<ImageReference>> FindReferences(Project project, bool searchCSharp, bool searchXaml, ImageReferenceConfiguration rules = null)
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

            rules ??= ImageReferenceConfiguration.Default;

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
                IReadOnlyList<ImageReference> fileReferences = FindReferences(file, project, rules);

                if (fileReferences != null && fileReferences.Any())
                {
                    references[file.FullName] = fileReferences;
                }
            }

            return references;
        }

        public static IReadOnlyList<ImageReference> FindReferences(FileInfo fileInfo, Project project, ImageReferenceConfiguration rules = null)
        {
            if (fileInfo is null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            rules ??= ImageReferenceConfiguration.Default;

            var fileContents = File.ReadAllText(fileInfo.FullName);

            return FindStringReferences(fileContents, fileInfo.FullName, project, rules);

        }

        private enum ParserState
        {
            /// <summary>
            /// The scanner is currently looking for " literals
            /// </summary>
            Scanning,

            /// <summary>
            /// The scanner is currently within a string literal and is building a string buffer.
            /// </summary>
            Building,
        }

        public static IReadOnlyList<ImageReference> FindStringReferences(string contents, string filePath, Project project, ImageReferenceConfiguration rules = null)
        {
            if (string.IsNullOrEmpty(contents))
            {
                throw new ArgumentException($"'{nameof(contents)}' cannot be null or empty.", nameof(contents));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or whitespace.", nameof(filePath));
            }

            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            rules ??= ImageReferenceConfiguration.Default;

            List<ImageReference> references = new List<ImageReference>();

            var parserState = ParserState.Scanning;

            var currentValueBuffer = "";

            for (var i = 0; i < contents.Length; i++)
            {
                var currentChar = contents[i];

                bool shouldCloseValue = false;
                switch (parserState)
                {
                    case ParserState.Scanning:
                        {
                            if (currentChar == StringLiteralCharacter)
                            {
                                parserState = ParserState.Building;
                            }
                        }
                        break;
                    case ParserState.Building:
                        {
                            if (currentChar == StringLiteralCharacter)
                            {
                                shouldCloseValue = CanCloseBuffer(currentValueBuffer);
                            }

                            if (!shouldCloseValue)
                            {
                                currentValueBuffer += currentChar;
                            }
                        }
                        break;
                }

                if (shouldCloseValue)
                {
                    if (TryParseToImageReference(currentValueBuffer, i, filePath, project, rules, out var imageReference))
                    {
                        references.Add(imageReference);
                    }

                    parserState = ParserState.Scanning;
                    currentValueBuffer = string.Empty;
                }
            }

            return references;
        }

        private static bool CanCloseBuffer(string currentValueBuffer)
        {
            if (currentValueBuffer.Length < 1)
            {
                return true;
            }

            var finalChar = currentValueBuffer.Last();

            if (finalChar == EscapeCharacter)
            {
                // Was the character before this escape character escaped?
                if (currentValueBuffer.Length > 1)
                {
                    var previousChar = currentValueBuffer[currentValueBuffer.Length - 2];
                    return previousChar == EscapeCharacter;
                }

                return false;
            }

            return true;
        }



        public static bool TryParseToImageReference(string value,
                                                    int currentSpanEnd,
                                                    string filePath,
                                                    Project project,
                                                    ImageReferenceConfiguration rules,
                                                    out ImageReference imageReference)
        {
            rules ??= ImageReferenceConfiguration.Default;
            imageReference = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (!Constants.ImageFileExtensions.Any(ext => value.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }

            if (ContainsStringFormatting(value) && rules.IgnoreFormattedStrings)
            {
                return false;
            }

            if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out _) && rules.IgnoreUrls)
            {
                return false;
            }

            var span = new TextSpan(currentSpanEnd - value.Length, value.Length);

            imageReference = new ImageReference(value, span, filePath, project.Name);

            return true;
        }

        private static bool ContainsStringFormatting(string contents)
        {
            if (string.IsNullOrWhiteSpace(contents))
            {
                return false;
            }

            var parserState = ParserState.Scanning;

            var currentValueBuffer = "";

            int formattedStringArgumentCount = 0;

            for (var i = 0; i < contents.Length; i++)
            {
                var currentChar = contents[i];

                bool shouldCloseValue = false;
                switch (parserState)
                {
                    case ParserState.Scanning:
                        {
                            if (currentChar == StringFormatStartCharacter)
                            {
                                parserState = ParserState.Building;
                            }
                        }
                        break;
                    case ParserState.Building:
                        {
                            if (currentChar == StringFormatEndCharacter)
                            {
                                shouldCloseValue = true;
                            }

                            if (!shouldCloseValue)
                            {
                                currentValueBuffer += currentChar;
                            }
                        }
                        break;
                }

                if (shouldCloseValue)
                {
                    if (int.TryParse(currentValueBuffer, out _))
                    {
                        formattedStringArgumentCount++;
                    }
                    parserState = ParserState.Scanning;
                    currentValueBuffer = string.Empty;
                }
            }

            return formattedStringArgumentCount > 0;
        }

        public static IReadOnlyList<string> GetFileExtensionsForSearch(bool searchCSharp, bool searchXaml)
        {
            var extensions = new List<string>();

            if (searchCSharp)
            {
                extensions.Add(Constants.CSharpFileExtension);
            }

            if (searchXaml)
            {
                extensions.Add(Constants.XamlFileExtension);
            }

            return extensions;
        }
    }
}

