using System;

namespace Known.Tests
{
    public sealed class Displayer
    {
        private static int passCount = 0;
        private static int failCount = 0;

        public static void Write(object message)
        {
            Console.Write(message);
        }

        public static void Write(ConsoleColor color, object message)
        {
            var orgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = orgColor;
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }

        public static void WriteLine(object message)
        {
            Console.WriteLine(message);
        }

        public static void WriteLine(ConsoleColor color, object message)
        {
            var orgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = orgColor;
        }

        public static void Write(bool pass)
        {
            if (pass)
            {
                passCount++;
                Write(ConsoleColor.Green, "Pass");
            }
            else
            {
                failCount++;
                Write(ConsoleColor.Red, "Fail");
            }
        }

        public static void DisplaySummary()
        {
            WriteLine(ConsoleColor.Yellow, new string('-', 100));
            WriteLine(ConsoleColor.Yellow, "|");
            Write(ConsoleColor.Yellow, $"|\tTotal Assert：{passCount + failCount}");
            Write(ConsoleColor.Green, $"    Pass：{passCount}");
            Write(ConsoleColor.Red, $"    Fail：{failCount}");
            WriteLine();
            WriteLine(ConsoleColor.Yellow, "|");
            WriteLine(ConsoleColor.Yellow, new string('-', 100));
            passCount = 0;
            failCount = 0;
        }
    }
}
