using System;
using System.Text.RegularExpressions;

namespace Known
{
    public sealed class TestAssert
    {
        public static void IsNull(object actual)
        {
            TestDisplayer.Write(actual == null);
            TestDisplayer.WriteLine($" 实际值：{actual}");
        }

        public static void IsNotNull(object actual)
        {
            TestDisplayer.Write(actual != null);
            TestDisplayer.WriteLine($" 实际值：{actual}");
        }

        public static void IsInstanceOf<T>(object value)
        {
            var expectedType = typeof(T);
            TestDisplayer.Write(expectedType.IsInstanceOfType(value));
            TestDisplayer.WriteLine($" 实际值：{value.GetType()}，期望值：{expectedType}");
        }

        public static void IsNotInstanceOf<T>(object value)
        {
            var wrongType = typeof(T);
            TestDisplayer.Write(!wrongType.IsInstanceOfType(value));
            TestDisplayer.WriteLine($" 实际值：{value.GetType()}，期望值：{wrongType}");
        }

        public static void IsInstanceOfType(object value, Type expectedType)
        {
            TestDisplayer.Write(expectedType.IsInstanceOfType(value));
            TestDisplayer.WriteLine($" 实际值：{value.GetType()}，期望值：{expectedType}");
        }

        public static void IsNotInstanceOfType(object value, Type wrongType)
        {
            TestDisplayer.Write(!wrongType.IsInstanceOfType(value));
            TestDisplayer.WriteLine($" 实际值：{value.GetType()}，期望值：{wrongType}");
        }

        public static void IsTrue(bool condition)
        {
            TestDisplayer.Write(condition);
            TestDisplayer.WriteLine($" 实际值：{condition}，期望值：true");
        }

        public static void IsFalse(bool condition)
        {
            TestDisplayer.Write(!condition);
            TestDisplayer.WriteLine($" 实际值：{condition}，期望值：false");
        }

        public static void AreEqual<T>(T actual, T expect)
        {
            TestDisplayer.Write(actual.Equals(expect));
            TestDisplayer.WriteLine($" 实际值：{actual}，期望值：{expect}");
        }

        public static void AreNotEqual<T>(T actual, T expect)
        {
            TestDisplayer.Write(!actual.Equals(expect));
            TestDisplayer.WriteLine($" 实际值：{actual}，期望值：{expect}");
        }

        public static void Contains(string value, string substring)
        {

        }

        public static void StartsWith(string value, string substring)
        {

        }

        public static void EndsWith(string value, string substring)
        {

        }

        public static void Matches(string value, Regex pattern)
        {

        }

        public static void DoesNotMatch(string value, Regex pattern)
        {

        }
    }

    public sealed class TestDisplayer
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
