using System;
using System.Collections.Generic;

namespace Known.Log
{
    /// <summary>
    /// 日志者抽象基类。
    /// </summary>
    public abstract class Logger : ILogger
    {
        private readonly List<string> traces = new List<string>();

        /// <summary>
        /// 取得日志跟踪信息。
        /// </summary>
        public string TraceInfo
        {
            get { return string.Join(Environment.NewLine, traces); }
        }

        /// <summary>
        /// 记录跟踪日志。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public void Trace(string message)
        {
            var info = WriteMessage(LogLevel.Info, message);
            traces.Add(info);
        }

        /// <summary>
        /// 记录信息日志。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public void Info(string message)
        {
            WriteMessage(LogLevel.Info, message);
        }

        /// <summary>
        /// 记录错误日志。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public void Error(string message)
        {
            WriteMessage(LogLevel.Error, message);
        }

        /// <summary>
        /// 记录异常日志。
        /// </summary>
        /// <param name="message">日志内容。</param>
        /// <param name="ex">异常对象。</param>
        public void Error(string message, Exception ex)
        {
            Error($"{message} {ex}");
        }

        /// <summary>
        /// 输出一行日志内容。
        /// </summary>
        /// <param name="level">日志级别。</param>
        /// <param name="message">日志内容。</param>
        protected abstract void WriteLine(LogLevel level, string message);

        private string WriteMessage(LogLevel level, string message)
        {
            var log = string.Format("{0:yyyy-MM-dd HH:mm:ss} {1}", DateTime.Now, message);
            WriteLine(level, $"{level} {log}");
            return log;
        }
    }
}
