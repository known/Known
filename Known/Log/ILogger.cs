using System;

namespace Known.Log
{
    /// <summary>
    /// 日志接口。
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 获取跟踪信息。
        /// </summary>
        string TraceInfo { get; }

        /// <summary>
        /// 跟踪日志内容。
        /// </summary>
        /// <param name="message">日志内容。</param>
        void Trace(string message);

        /// <summary>
        /// 跟踪格式化的日志内容。
        /// </summary>
        /// <param name="format">格式化。</param>
        /// <param name="args">参数。</param>
        void Trace(string format, params object[] args);

        /// <summary>
        /// 记录信息级别的日志内容。
        /// </summary>
        /// <param name="message">日志内容。</param>
        void Info(string message);

        /// <summary>
        /// 记录格式化的信息级别的日志内容。
        /// </summary>
        /// <param name="format">格式化。</param>
        /// <param name="args">参数。</param>
        void Info(string format, params object[] args);

        /// <summary>
        /// 记录错误级别的日志内容。
        /// </summary>
        /// <param name="message">日志内容。</param>
        void Error(string message);

        /// <summary>
        /// 记录格式化的错误级别的日志内容。
        /// </summary>
        /// <param name="format">格式化。</param>
        /// <param name="args">参数。</param>
        void Error(string format, params object[] args);

        /// <summary>
        /// 记录错误级别的异常信息。
        /// </summary>
        /// <param name="message">异常描述。</param>
        /// <param name="ex">异常。</param>
        void Error(string message, Exception ex);
    }
}
