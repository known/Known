using System;
using System.Text;
using Known.Extensions;

namespace Known.Tests.Core.Extensions
{
    public class StringExtensionTest
    {
        public static void TestAppendLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{0}不能为空！", "Item1");
            Assert.IsEqual(sb.ToString(), "Item1不能为空！" + Environment.NewLine);
        }

        public static void TestByteLength()
        {
            var value = "Item1不能为空！";
            Assert.IsEqual(value.ByteLength(), 15);
        }

        public static void TestByteSubstring()
        {
            var value = "Item1不能为空！";
            Assert.IsEqual(value.ByteSubstring(5), "不能为空！");
        }

        public static void TestByteSubstringWithLength()
        {
            var value = "Item1不能为空！";
            Assert.IsEqual(value.ByteSubstring(5, 4), "不能");
        }

        public static void TestFormatXml()
        {
            var value = new TestEntity { Item1 = 1, Item2 = "test", Item3 = new DateTime(2017, 10, 1) };
            var value1 = new TestEntity { Item1 = 1, Item2 = "test", Item3 = new DateTime(2017, 10, 1) };
            var valueXml = value.ToXml().Replace("  ", "").Replace(Environment.NewLine, "");
            Assert.IsEqual(valueXml.FormatXml(), value1.ToXml());
        }
    }
}
