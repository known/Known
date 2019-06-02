using System;
using System.Diagnostics;
using System.IO;

namespace Known.Log
{
    /// <summary>
    /// 跟踪日志者。
    /// </summary>
    public class TraceLogger : Logger, ILogger
    {
        /// <summary>
        /// 初始化一个跟踪日志者实例，日志路径默认为当前工作目录 logs 文件下。
        /// </summary>
        public TraceLogger()
        {
            var fileName = Path.Combine(Environment.CurrentDirectory, "logs", DateTime.Now.ToString("yyyyMMdd") + ".log");
            Initialize(fileName);
        }

        /// <summary>
        /// 初始化一个指定路径的跟踪日志者实例。
        /// </summary>
        /// <param name="fileName">日志文件路径。</param>
        public TraceLogger(string fileName)
        {
            Initialize(fileName);
        }

        /// <summary>
        /// 输出一行日志内容。
        /// </summary>
        /// <param name="level">日志级别。</param>
        /// <param name="message">日志内容。</param>
        protected override void WriteLine(LogLevel level, string message)
        {
            System.Diagnostics.Trace.TraceInformation(message);
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
    }
}
