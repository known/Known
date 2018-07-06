using System;
using System.Diagnostics;
using System.IO;

namespace Known.Log
{
    public class TraceLogger : Logger, ILogger
    {
        public TraceLogger(string logPath)
        {
            var fileName = Path.Combine(logPath, "logs", DateTime.Now.ToString("yyyyMMdd") + ".log");
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            System.Diagnostics.Trace.AutoFlush = true;
            System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(fileName));
        }

        protected override void WriteLine(LogLevel level, string message)
        {
            System.Diagnostics.Trace.TraceInformation(message);
        }
    }
}
