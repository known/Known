using System;

namespace Known.Tests
{
    public sealed class Assert
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

        public static void IsInstanceOf<T>(object value)
        {
            var expectedType = typeof(T);
            Write(expectedType.IsInstanceOfType(value));
            Displayer.WriteLine($" 实际值：{value.GetType()}，期望值：{expectedType}");
        }

        public static void IsNotInstanceOf<T>(object value)
        {
            var wrongType = typeof(T);
            Write(!wrongType.IsInstanceOfType(value));
            Displayer.WriteLine($" 实际值：{value.GetType()}，期望值：{wrongType}");
        }

        public static void IsInstanceOfType(object value, Type expectedType)
        {
            Write(expectedType.IsInstanceOfType(value));
            Displayer.WriteLine($" 实际值：{value.GetType()}，期望值：{expectedType}");
        }

        public static void IsNotInstanceOfType(object value, Type wrongType)
        {
            Write(!wrongType.IsInstanceOfType(value));
            Displayer.WriteLine($" 实际值：{value.GetType()}，期望值：{wrongType}");
        }

        public static void IsTrue(bool condition)
        {
            Write(condition);
            Displayer.WriteLine($" 实际值：{condition}，期望值：true");
        }

        public static void IsFalse(bool condition)
        {
            Write(!condition);
            Displayer.WriteLine($" 实际值：{condition}，期望值：false");
        }

        public static void AreEqual<T>(T actual, T expect)
        {
            Write(actual.Equals(expect));
            Displayer.WriteLine($" 实际值：{actual}，期望值：{expect}");
        }

        public static void AreNotEqual<T>(T actual, T expect)
        {
            Write(!actual.Equals(expect));
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
