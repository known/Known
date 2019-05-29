using System;

namespace Known.Log
{
    /// <summary>
    /// 日志者接口。
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 取得日志跟踪信息。
        /// </summary>
        string TraceInfo { get; }

        /// <summary>
        /// 记录跟踪日志。
        /// </summary>
        /// <param name="message">日志内容。</param>
        void Trace(string message);

        /// <summary>
        /// 记录信息日志。
        /// </summary>
        /// <param name="message">日志内容。</param>
        void Info(string message);

        /// <summary>
        /// 记录错误日志。
        /// </summary>
        /// <param name="message">日志内容。</param>
        void Error(string message);

        /// <summary>
        /// 记录异常日志。
        /// </summary>
        /// <param name="message">日志内容。</param>
        /// <param name="ex">异常对象。</param>
        void Error(string message, Exception ex);
    }
}
