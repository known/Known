using System;
using System.Collections.Generic;
using System.Threading;

namespace Known.Log
{
    class LogInfo
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// 日志帮助者。
    /// </summary>
    public class LogHelper
    {
        private static readonly Queue<LogInfo> logs = new Queue<LogInfo>();
        private static readonly Dictionary<long, DateTime> hashObjs = new Dictionary<long, DateTime>();
        private static bool threadWorking = false;

        /// <summary>
        /// 写信息。
        /// </summary>
        /// <param name="message">日志信息。</param>
        public static void Info(string message)
        {
            Write(LogLevel.Info, message);
        }

        /// <summary>
        /// 写错误信息。
        /// </summary>
        /// <param name="message">日志信息。</param>
        public static void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        private static void Write(LogLevel level, string message)
        {
            var key = message.GetHashCode();
            if (hashObjs.ContainsKey(key))
                return;

            hashObjs.Add(key, DateTime.Now);
            logs.Enqueue(new LogInfo { Level = level, Message = message });
            if (!threadWorking)
            {
                threadWorking = true;
                ThreadPool.QueueUserWorkItem(DoWrite, null);
            }
        }

        private static void DoWrite(object p)
        {
            var logger = new FileLogger();
            var num = 0;
            do
            {
                num++;
                while (logs.Count > 0)
                {
                    num = 0;
                    var log = logs.Dequeue();
                    switch (log.Level)
                    {
                        case LogLevel.Info:
                            logger.Info(log.Message);
                            break;
                        case LogLevel.Error:
                            logger.Error(log.Message);
                            break;
                        default:
                            break;
                    }
                }
                Thread.Sleep(5000);
            }
            while (num <= 100);

            hashObjs.Clear();
            threadWorking = false;
        }
    }
}
