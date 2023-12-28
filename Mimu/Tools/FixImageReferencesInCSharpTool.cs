using System;
using Mimu.Models;

namespace Mimu.Tools
{
	public class FixImageReferencesInCSharpTool : ITool
    {
        public string Name => "fix-csharp-references";

        public void Run(IReadOnlyList<Project> projects)
        {
        }
    }
}