using System;

namespace Known.Tests
{
    public sealed class Assert
    {
        public static void IsNull(object actual)
        {
            Displayer.Write(actual == null);
            Displayer.WriteLine($" 实际值：{actual}");
        }

        public static void IsNotNull(object actual)
        {
            Displayer.Write(actual != null);
            Displayer.WriteLine($" 实际值：{actual}");
        }

        public static void IsInstanceOf<T>(object value)
        {
            var expectedType = typeof(T);
            Displayer.Write(expectedType.IsInstanceOfType(value));
            Displayer.WriteLine($" 实际值：{value.GetType()}，期望值：{expectedType}");
        }

        public static void IsNotInstanceOf<T>(object value)
        {
            var wrongType = typeof(T);
            Displayer.Write(!wrongType.IsInstanceOfType(value));
            Displayer.WriteLine($" 实际值：{value.GetType()}，期望值：{wrongType}");
        }

        public static void IsInstanceOfType(object value, Type expectedType)
        {
            Displayer.Write(expectedType.IsInstanceOfType(value));
            Displayer.WriteLine($" 实际值：{value.GetType()}，期望值：{expectedType}");
        }

        public static void IsNotInstanceOfType(object value, Type wrongType)
        {
            Displayer.Write(!wrongType.IsInstanceOfType(value));
            Displayer.WriteLine($" 实际值：{value.GetType()}，期望值：{wrongType}");
        }

        public static void IsTrue(bool condition)
        {
            Displayer.Write(condition);
            Displayer.WriteLine($" 实际值：{condition}，期望值：true");
        }

        public static void IsFalse(bool condition)
        {
            Displayer.Write(!condition);
            Displayer.WriteLine($" 实际值：{condition}，期望值：false");
        }

        public static void AreEqual<T>(T actual, T expect)
        {
            Displayer.Write(actual.Equals(expect));
            Displayer.WriteLine($" 实际值：{actual}，期望值：{expect}");
        }

        public static void AreNotEqual<T>(T actual, T expect)
        {
            Displayer.Write(!actual.Equals(expect));
            Displayer.WriteLine($" 实际值：{actual}，期望值：{expect}");
        }
    }
}
