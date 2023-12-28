using System;
using Mimu.Models;

namespace Mimu.Tools
{
	public interface ITool
	{
		string Name { get; }

		void Run(IReadOnlyList<Project> projects);
	}
}

