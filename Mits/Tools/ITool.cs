using System;
using Mits.Models;

namespace Mits.Tools
{
	public interface ITool
	{
		string Name { get; }

		void Run(IReadOnlyList<Project> projects);
	}
}

