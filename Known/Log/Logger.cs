using System;
using System.Collections.Generic;

namespace Known.Log
{
    /// <summary>
    /// 抽象的日志基类。
    /// </summary>
    public abstract class Logger : ILogger
    {
        private List<string> traces = new List<string>();

        /// <summary>
        /// 获取跟踪信息。
        /// </summary>
        public string TraceInfo
        {
            get { return string.Join(Environment.NewLine, traces); }
        }

        /// <summary>
        /// 跟踪日志内容。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public void Trace(string message)
        {
            var info = WriteMessage(LogLevel.Info, message);
            traces.Add(info);
        }

        /// <summary>
        /// 跟踪格式化的日志内容。
        /// </summary>
        /// <param name="format">格式化。</param>
        /// <param name="args">参数。</param>
        public void Trace(string format, params object[] args)
        {
            Trace(string.Format(format, args));
        }

        /// <summary>
        /// 记录信息级别的日志内容。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public void Info(string message)
        {
            WriteMessage(LogLevel.Info, message);
        }

        /// <summary>
        /// 记录格式化的信息级别的日志内容。
        /// </summary>
        /// <param name="format">格式化。</param>
        /// <param name="args">参数。</param>
        public void Info(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }

        /// <summary>
        /// 记录错误级别的日志内容。
        /// </summary>
        /// <param name="message">日志内容。</param>
        public void Error(string message)
        {
            WriteMessage(LogLevel.Error, message);
        }

        /// <summary>
        /// 记录格式化的错误级别的日志内容。
        /// </summary>
        /// <param name="format">格式化。</param>
        /// <param name="args">参数。</param>
        public void Error(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }

        /// <summary>
        /// 记录错误级别的异常信息。
        /// </summary>
        /// <param name="message">异常描述。</param>
        /// <param name="ex">异常。</param>
        public void Error(string message, Exception ex)
        {
            Error($"{message} {ex}");
        }

        /// <summary>
        /// 写入单行日志内容。
        /// </summary>
        /// <param name="level">日志级别。</param>
        /// <param name="message">日志内容。</param>
        protected abstract void WriteLine(LogLevel level, string message);

        private string WriteMessage(LogLevel level, string message)
        {
            var log = string.Format("{0:yyyy-MM-dd HH:mm:ss fff} {1}", DateTime.Now, message);
            WriteLine(level, $"{level} {log}");
            return log;
        }
    }
}
