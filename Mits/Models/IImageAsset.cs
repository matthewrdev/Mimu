using System;
namespace Mits.Models
{
	public interface IImageAsset
	{
		string Name { get; }

		string FilePath { get; }

		string Extension { get; }
	}
}

