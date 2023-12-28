﻿using System.Collections.Concurrent;
using Mits.Utilities;

namespace Mits.Logging
{
    public class LogFileWriter : IDisposable
    {
        readonly ConcurrentQueue<string> outputList;
        readonly Thread loggerThread;
        readonly AutoResetEvent @event;

        private readonly ConcurrentValue<bool> closed = new ConcurrentValue<bool>(false);
        bool Closed
        {
            get => closed.Get();
            set => closed.Set(value);
        }

        public string LogFilePath { get; }

        public LogFileWriter(string logFilePath)
        {
            if (string.IsNullOrEmpty(logFilePath))
            {
                throw new ArgumentException($"'{nameof(logFilePath)}' cannot be null or empty.", nameof(logFilePath));
            }

            LogFilePath = logFilePath;

            outputList = new ConcurrentQueue<string>();
            loggerThread = new Thread(new ThreadStart(this.LoggerWorker));
            @event = new AutoResetEvent(false);

            loggerThread.Start();
        }

        public void Dispose()
        {
            Closed = true;
            @event.Set();
            loggerThread.Join();
        }

        public void WriteToFile(string output)
        {
            outputList.Enqueue(output);
            @event.Set();
        }

        void LoggerWorker()
        {
            while (!Closed)
            {
                @event.WaitOne();
                while (outputList.IsEmpty == false && !Closed)
                {
                    if (outputList.TryDequeue(out var output))
                    {
                        File.AppendAllText(LogFilePath, output);
                    }
                }
            }
        }
    }
}

