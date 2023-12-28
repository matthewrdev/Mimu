using System;
using Mits.Models;

namespace Mits.Tools
{
	public interface ITool
	{
		string Name { get; }

		string Help { get; }

		// TODO: Configuratoin object?

		void Run(ToolConfiguration config);
	}
}

