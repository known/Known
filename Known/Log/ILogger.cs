using System;

namespace Known.Log
{
    public interface ILogger
    {
        string TraceInfo { get; }

        void Trace(string message);
        void Trace(string format, params object[] args);
        void Info(string message);
        void Info(string format, params object[] args);
        void Error(string message);
        void Error(string format, params object[] args);
        void Error(string message, Exception ex);
    }
}
