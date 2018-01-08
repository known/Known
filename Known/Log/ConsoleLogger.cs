using System;

namespace Known.Log
{
    /// <summary>
    /// 控制台日志类。
    /// </summary>
    public class ConsoleLogger : Logger, ILogger
    {
        /// <summary>
        /// 写入单行日志内容。
        /// </summary>
        /// <param name="level">日志级别。</param>
        /// <param name="message">日志内容。</param>
        protected override void WriteLine(LogLevel level, string message)
        {
            var orgColor = Console.ForegroundColor;
            switch (level)
            {
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.WriteLine(message);
            Console.ForegroundColor = orgColor;
        }
    }
}
