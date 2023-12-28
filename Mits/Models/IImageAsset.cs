using System;
using System.Drawing;

namespace Mits.Models
{
	public interface IImageAsset
	{
		string Name { get; }

		string CompatName { get; }

		string FilePath { get; }

		string Extension { get; }

		Project Project { get; }

		Size Size { get; }
	}
}

