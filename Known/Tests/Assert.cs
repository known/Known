using System;

namespace Known.Tests
{
    public class Assert
    {
        private static int passCount = 0;
        private static int failCount = 0;

        public static void IsNull(object actual)
        {
            Write(actual == null);
            Displayer.WriteLine($" 实际值：{actual}");
        }

        public static void IsNotNull(object actual)
        {
            Write(actual != null);
            Displayer.WriteLine($" 实际值：{actual}");
        }

        public static void AreEqual<T>(T actual, T expect)
        {
            Write(actual.Equals(expect));
            Displayer.WriteLine($" 实际值：{actual}，期望值：{expect}");
        }

        public static void DisplaySummary()
        {
            Displayer.WriteLine(ConsoleColor.Yellow, new string('-', 100));
            Displayer.WriteLine(ConsoleColor.Yellow, "|");
            Displayer.Write(ConsoleColor.Yellow, $"|\tTotal Assert：{passCount + failCount}");
            Displayer.Write(ConsoleColor.Green, $"    Pass：{passCount}");
            Displayer.Write(ConsoleColor.Red, $"    Fail：{failCount}");
            Displayer.WriteLine();
            Displayer.WriteLine(ConsoleColor.Yellow, "|");
            Displayer.WriteLine(ConsoleColor.Yellow, new string('-', 100));
            passCount = 0;
            failCount = 0;
        }

        private static void Write(bool pass)
        {
            if (pass)
            {
                passCount++;
                Displayer.Write(ConsoleColor.Green, "Pass");
            }
            else
            {
                failCount++;
                Displayer.Write(ConsoleColor.Red, "Fail");
            }
        }
    }
}
