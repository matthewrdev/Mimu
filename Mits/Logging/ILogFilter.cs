using System;
using System.Collections.Generic;

namespace Mits.Logging
{
    public interface ILogFilter
    {
        bool CanLog(string tag, string message, LogLevel logLevel);
    }
}

