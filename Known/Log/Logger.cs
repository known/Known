using System;
using System.Collections.Generic;

namespace Known.Log
{
    public abstract class Logger : ILogger
    {
        private List<string> traces = new List<string>();

        public string TraceInfo
        {
            get { return string.Join(Environment.NewLine, traces); }
        }

        public void Trace(string message)
        {
            var info = WriteMessage(LogLevel.Info, message);
            traces.Add(info);
        }

        public void Trace(string format, params object[] args)
        {
            Trace(string.Format(format, args));
        }

        public void Info(string message)
        {
            WriteMessage(LogLevel.Info, message);
        }

        public void Info(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }

        public void Error(string message)
        {
            WriteMessage(LogLevel.Error, message);
        }

        public void Error(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }

        public void Error(string message, Exception ex)
        {
            Error($"{message} {ex}");
        }

        protected abstract void WriteLine(LogLevel level, string message);

        private string WriteMessage(LogLevel level, string message)
        {
            var log = string.Format("{0:yyyy-MM-dd HH:mm:ss} {1}", DateTime.Now, message);
            WriteLine(level, $"{level} {log}");
            return log;
        }
    }
}
