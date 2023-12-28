using System;
using Mits.Models;

namespace Mits.Tools
{
	public class FixImageReferencesInCSharpTool : ITool
    {
        public string Name => "fix-csharp-references";

        public void Run(IReadOnlyList<Project> projects)
        {
        }
    }
}