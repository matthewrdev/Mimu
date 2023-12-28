using System;
using Mits.Utilities;

namespace Mits.Logging
{
    public abstract class BaseLogger : ILogger
    {
        public BaseLogger(string tag)
        {
            Tag = tag;
        }

        public void Error(string message)
        {
            Log(Tag, message, LogLevel.Error);
        }

        public void Warning(string message)
        {
            Log(Tag, message, LogLevel.Warning);
        }

        public void Info(string message)
        {
            Log(Tag, message, LogLevel.Information);
        }

        public void Debug(string message)
        {
            Log(Tag, message, LogLevel.Debug);
        }

        public void Verbose(string message)
        {
            Log(Tag, message, LogLevel.Verbose);
        }

        public void Property(string key, string value, string group = "")
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"'{nameof(value)}' cannot be null or whitespace.", nameof(value));
            }

            var tag = Tags.PropertyTag;
            if (!string.IsNullOrEmpty(group))
            {
                tag = $"{tag}.{group.Trim()}";
            }

            Log(tag.RemoveWhitespace(), $"{key.Trim()}='{value}'", LogLevel.Information);
        }

        public string Tag { get; }

        public abstract void Log(string tag, string message, LogLevel logLevel = LogLevel.Information);

        public abstract void Exception(Exception ex);

        public abstract void Event(string eventName, IReadOnlyDictionary<string, object> properties = null);

        public abstract void Event(string eventName, params (string parameterName, object parameterValue)[] parameters);
    }
}

