using System;

namespace Known.Tests
{
    public class Assert
    {
        private static int passCount = 0;
        private static int failCount = 0;

        public static void IsNotNull(string actual)
        {
            Write(!string.IsNullOrWhiteSpace(actual));
            Console.WriteLine($" 实际值：{actual}");
        }

        public static void IsEqual<T>(T actual, T expect)
        {
            Write(actual.Equals(expect));
            Console.WriteLine($" 实际值：{actual}，期望值：{expect}");
        }

        public static void DisplaySummary()
        {
            var orgColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('-', 100));
            Console.WriteLine("|");
            Console.Write("|\tTotal Assert：{0}", passCount + failCount);
            Write(ConsoleColor.Green, $"    Pass：{passCount}");
            Write(ConsoleColor.Red, $"    Fail：{failCount}");
            Console.WriteLine();
            Console.WriteLine("|");
            Console.WriteLine(new string('-', 100));
            Console.ForegroundColor = orgColor;
            passCount = 0;
            failCount = 0;
        }

        private static void Write(bool pass)
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

        public static void Write(ConsoleColor color, object message)
        {
            var orgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = orgColor;
        }

        public static void WriteLine(ConsoleColor color, object message)
        {
            var orgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = orgColor;
        }
    }
}
