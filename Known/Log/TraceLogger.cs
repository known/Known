using System;
using System.Diagnostics;
using System.IO;

namespace Known.Log
{
    /// <summary>
    /// 跟踪日志类。
    /// </summary>
    public class TraceLogger : Logger, ILogger
    {
        /// <summary>
        /// 构造函数，创建一个跟踪日志类实例。
        /// </summary>
        /// <param name="logPath">日志文件夹路径。</param>
        public TraceLogger(string logPath)
        {
            var fileName = Path.Combine(logPath, "logs", DateTime.Now.ToString("yyyyMMdd") + ".log");
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            System.Diagnostics.Trace.AutoFlush = true;
            System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(fileName));
        }

        /// <summary>
        /// 写入单行日志内容。
        /// </summary>
        /// <param name="level">日志级别。</param>
        /// <param name="message">日志内容。</param>
        protected override void WriteLine(LogLevel level, string message)
        {
            System.Diagnostics.Trace.TraceInformation(message);
        }
    }
}
