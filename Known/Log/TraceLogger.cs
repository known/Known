using System;
using System.Diagnostics;
using System.IO;

namespace Known.Log
{
    public class TraceLogger : Logger, ILogger
    {
        public TraceLogger()
        {
            var fileName = Path.Combine(Environment.CurrentDirectory, "logs", DateTime.Now.ToString("yyyyMMdd") + ".log");
            Initialize(fileName);
        }

        public TraceLogger(string fileName)
        {
            Initialize(fileName);
        }

        private void Initialize(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            System.Diagnostics.Trace.AutoFlush = true;
            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(fileName));
        }

        protected override void WriteLine(LogLevel level, string message)
        {
            System.Diagnostics.Trace.TraceInformation(message);
        }
    }
}
