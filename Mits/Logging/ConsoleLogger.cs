using System;
using System.Diagnostics;
using System.Threading;

namespace Mits.Logging
{
    public class ConsoleLogger : BaseLogger
    {
        public int ProcessId { get; }

        public ConsoleLogger(string context)
            : base(context)
        {
            ProcessId = Process.GetCurrentProcess().Id;
        }

        public override void Log(string tag, string message, LogLevel logLevel)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

#if ANDROID
            LogAndroid(tag, message, logLevel);
#else
            var now = DateTime.Now;
            var lines = message.Split('\n', '\r');

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var output = LogFormatter.Render(now, tag, line, ProcessId, Thread.CurrentThread.ManagedThreadId, logLevel);
                Console.Out.WriteLine(output);
            }
#endif
        }

#if ANDROID
        private void LogAndroid(string tag, string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Verbose:
                    Android.Util.Log.Verbose(tag, message);
                    break;
                case LogLevel.Debug:
                    Android.Util.Log.Debug(tag, message);
                    break;
                case LogLevel.Information:
                    Android.Util.Log.Info(tag, message);
                    break;
                case LogLevel.Warning:
                    Android.Util.Log.Warn(tag, message);
                    break;
                case LogLevel.Error:
                    Android.Util.Log.Error(tag, message);
                    break;
                case LogLevel.Fatal:
                    Android.Util.Log.Wtf(tag, message);
                    break;
            }
        }
#endif

        public override void Exception(Exception ex)
        {
            Log(Tag, ex.ToString(), LogLevel.Error);
        }

        public override void Event(string eventName, IReadOnlyDictionary<string, object> properties = null)
        {
        }

        public override void Event(string eventName, params (string parameterName, object parameterValue)[] parameters)
        {
        }
    }
}

