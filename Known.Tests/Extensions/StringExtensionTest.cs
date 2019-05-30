using System;
using System.Text;
using Known.Extensions;

namespace Known.Tests.Extensions
{
    public class StringExtensionTest
    {
        public static void AppendLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{0}不能为空！", "Item1");
            TestAssert.AreEqual(sb.ToString(), "Item1不能为空！" + Environment.NewLine);
        }

        public static void ByteLength()
        {
            var value = "Item1不能为空！";
            TestAssert.AreEqual(value.ByteLength(), 15);
        }

        public static void ByteSubstring()
        {
            var value = "Item1不能为空！";
            TestAssert.AreEqual(value.ByteSubstring(5), "不能为空！");
        }

        public static void ByteSubstringWithLength()
        {
            var value = "Item1不能为空！";
            TestAssert.AreEqual(value.ByteSubstring(5, 4), "不能");
        }

        public static void FormatXml()
        {
            var value = new TestEntity { Id = "1", Item1 = 1, Item2 = "test", Item3 = new DateTime(2017, 10, 1) };
            var value1 = new TestEntity { Id = "1", Item1 = 1, Item2 = "test", Item3 = new DateTime(2017, 10, 1) };
            var valueXml = value.ToXml().Replace("  ", "").Replace(Environment.NewLine, "");
            TestAssert.AreEqual(valueXml.FormatXml(), value1.ToXml());
        }
    }
}
