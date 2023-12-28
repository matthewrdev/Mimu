using System;

namespace Mits.Logging
{
    public interface ILogFileProvider
    {
        string LogDirectory { get; }

        string CurrentLogFile { get; }
    }
}