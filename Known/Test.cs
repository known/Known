using System;
using System.Text.RegularExpressions;

namespace Known
{
    /// <summary>
    /// 测试断言类。
    /// </summary>
    public sealed class TestAssert
    {
        /// <summary>
        /// 判断对象是否为 null。
        /// </summary>
        /// <param name="actual">实际对象。</param>
        public static void IsNull(object actual)
        {
            TestDisplayer.Write(actual == null);
            TestDisplayer.WriteLine($" 实际值：{actual}");
        }

        /// <summary>
        /// 判断对象是否不为 null。
        /// </summary>
        /// <param name="actual">实际对象。</param>
        public static void IsNotNull(object actual)
        {
            TestDisplayer.Write(actual != null);
            TestDisplayer.WriteLine($" 实际值：{actual}");
        }

        /// <summary>
        /// 判断对象是否指定泛型类型的实例。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="value">实际对象。</param>
        public static void IsInstanceOf<T>(object value)
        {
            var expectedType = typeof(T);
            TestDisplayer.Write(expectedType.IsInstanceOfType(value));
            TestDisplayer.WriteLine($" 实际值：{value.GetType()}，期望值：{expectedType}");
        }

        /// <summary>
        /// 判断对象是否不是指定泛型类型的实例。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="value">实际对象。</param>
        public static void IsNotInstanceOf<T>(object value)
        {
            var wrongType = typeof(T);
            TestDisplayer.Write(!wrongType.IsInstanceOfType(value));
            TestDisplayer.WriteLine($" 实际值：{value.GetType()}，期望值：{wrongType}");
        }

        /// <summary>
        /// 判断对象是否指定类型的实例。
        /// </summary>
        /// <param name="value">实际对象。</param>
        /// <param name="expectedType">对象类型。</param>
        public static void IsInstanceOfType(object value, Type expectedType)
        {
            TestDisplayer.Write(expectedType.IsInstanceOfType(value));
            TestDisplayer.WriteLine($" 实际值：{value.GetType()}，期望值：{expectedType}");
        }

        /// <summary>
        /// 判断对象是否不是指定类型的实例。
        /// </summary>
        /// <param name="value">实际对象。</param>
        /// <param name="wrongType">对象类型。</param>
        public static void IsNotInstanceOfType(object value, Type wrongType)
        {
            TestDisplayer.Write(!wrongType.IsInstanceOfType(value));
            TestDisplayer.WriteLine($" 实际值：{value.GetType()}，期望值：{wrongType}");
        }

        /// <summary>
        /// 判断条件是否为真。
        /// </summary>
        /// <param name="condition">断言条件。</param>
        public static void IsTrue(bool condition)
        {
            TestDisplayer.Write(condition);
            TestDisplayer.WriteLine($" 实际值：{condition}，期望值：true");
        }

        /// <summary>
        /// 判断条件是否为假。
        /// </summary>
        /// <param name="condition">断言条件。</param>
        public static void IsFalse(bool condition)
        {
            TestDisplayer.Write(!condition);
            TestDisplayer.WriteLine($" 实际值：{condition}，期望值：false");
        }

        /// <summary>
        /// 判断对象是否与期望值相等。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="actual">实际对象。</param>
        /// <param name="expect">期望对象。</param>
        public static void AreEqual<T>(T actual, T expect)
        {
            TestDisplayer.Write(actual.Equals(expect));
            TestDisplayer.WriteLine($" 实际值：{actual}，期望值：{expect}");
        }

        /// <summary>
        /// 判断对象是否与期望值不相等。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="actual">实际对象。</param>
        /// <param name="expect">期望对象。</param>
        public static void AreNotEqual<T>(T actual, T expect)
        {
            TestDisplayer.Write(!actual.Equals(expect));
            TestDisplayer.WriteLine($" 实际值：{actual}，期望值：{expect}");
        }

        /// <summary>
        /// 判断字符串是否包含指定的子字符串。
        /// </summary>
        /// <param name="value">字符串。</param>
        /// <param name="substring">子字符串。</param>
        public static void Contains(string value, string substring)
        {
            IsTrue(value.Contains(substring));
        }

        /// <summary>
        /// 判断字符串是否是以指定子字符串开头。
        /// </summary>
        /// <param name="value">字符串。</param>
        /// <param name="substring">子字符串。</param>
        public static void StartsWith(string value, string substring)
        {
            IsTrue(value.StartsWith(substring));
        }

        /// <summary>
        /// 判断字符串是否是以指定子字符串结尾。
        /// </summary>
        /// <param name="value">字符串。</param>
        /// <param name="substring">子字符串。</param>
        public static void EndsWith(string value, string substring)
        {
            IsTrue(value.EndsWith(substring));
        }

        /// <summary>
        /// 判断字符串是否匹配指定正则表达式。
        /// </summary>
        /// <param name="value">字符串。</param>
        /// <param name="pattern">正则表达式。</param>
        public static void Matches(string value, Regex pattern)
        {
            IsTrue(pattern.IsMatch(value));
        }

        /// <summary>
        /// 判断字符串是否不匹配指定正则表达式。
        /// </summary>
        /// <param name="value">字符串。</param>
        /// <param name="pattern">正则表达式。</param>
        public static void DoesNotMatch(string value, Regex pattern)
        {
            IsTrue(!pattern.IsMatch(value));
        }
    }

    /// <summary>
    /// 测试结果显示类。
    /// </summary>
    public sealed class TestDisplayer
    {
        private static int passCount = 0;
        private static int failCount = 0;

        /// <summary>
        /// 输出消息内容。
        /// </summary>
        /// <param name="message">消息内容。</param>
        public static void Write(object message)
        {
            Console.Write(message);
        }

        /// <summary>
        /// 输出指定颜色的消息内容。
        /// </summary>
        /// <param name="color">消息颜色。</param>
        /// <param name="message">消息内容。</param>
        public static void Write(ConsoleColor color, object message)
        {
            var orgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = orgColor;
        }

        /// <summary>
        /// 输出一个空行。
        /// </summary>
        public static void WriteLine()
        {
            Console.WriteLine();
        }

        /// <summary>
        /// 输出换新行的消息内容。
        /// </summary>
        /// <param name="message">消息内容。</param>
        public static void WriteLine(object message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// 输出指定颜色并换新行的消息内容。
        /// </summary>
        /// <param name="color">消息颜色。</param>
        /// <param name="message">消息内容。</param>
        public static void WriteLine(ConsoleColor color, object message)
        {
            var orgColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = orgColor;
        }

        /// <summary>
        /// 输出测试断言信息。
        /// </summary>
        /// <param name="pass">断言结果。</param>
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

        /// <summary>
        /// 显示测试结果摘要信息。
        /// </summary>
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
