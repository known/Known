using System;

namespace Known.Log
{
    public class ConsoleLogger : Logger, ILogger
    {
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
