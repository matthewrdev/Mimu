using System;
using Mimu.Models;

namespace Mimu.Tools
{
    public class FixImageReferencesInXamlTool : ITool
    {
        public string Name => "fix-xaml-references";

        public void Run(IReadOnlyList<Project> projects)
        {
        }
    }
}

