using System;

namespace Mits.Logging
{
	public interface ILoggerFactory : IDisposable
	{
		ILogger Create(string tag);
	}
}

