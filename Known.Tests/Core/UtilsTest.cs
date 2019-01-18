using System;
using System.IO;

namespace Known.Tests.Core
{
    public class UtilsTest
    {
        public static void NewGuid()
        {
            TestAssert.AreEqual(Utils.NewGuid.Length, 32);
        }

        public static void IsNullOrEmpty()
        {
            TestAssert.AreEqual(Utils.IsNullOrEmpty(null), true);
            TestAssert.AreEqual(Utils.IsNullOrEmpty(DBNull.Value), true);
            TestAssert.AreEqual(Utils.IsNullOrEmpty(""), true);
            TestAssert.AreEqual(Utils.IsNullOrEmpty(" "), true);
            TestAssert.AreEqual(Utils.IsNullOrEmpty("test"), false);
        }

        public static void ConvertTo()
        {
            TestAssert.AreEqual(Utils.ConvertTo<int>("1"), 1);
            TestAssert.AreEqual(Utils.ConvertTo<decimal>("1.2"), 1.2M);
            TestAssert.AreEqual(Utils.ConvertTo<bool>(1), true);
            TestAssert.AreEqual(Utils.ConvertTo<bool>(0), false);
            TestAssert.AreEqual(Utils.ConvertTo<bool>("Y"), true);
            TestAssert.AreEqual(Utils.ConvertTo<bool>("y"), true);
            TestAssert.AreEqual(Utils.ConvertTo<bool>("Yes"), true);
            TestAssert.AreEqual(Utils.ConvertTo<bool>("yes"), true);
            TestAssert.AreEqual(Utils.ConvertTo<TestEnum>(0), TestEnum.Enum1);
        }

        public static void ConvertToWithType()
        {
            TestAssert.AreEqual(Utils.ConvertTo(typeof(int), "1", 0), 1);
            TestAssert.AreEqual(Utils.ConvertTo(typeof(decimal), "1.2", 0), 1.2M);
            TestAssert.AreEqual(Utils.ConvertTo(typeof(bool), 1, false), true);
            TestAssert.AreEqual(Utils.ConvertTo(typeof(bool), 0, false), false);
            TestAssert.AreEqual(Utils.ConvertTo(typeof(bool), "Y", false), true);
            TestAssert.AreEqual(Utils.ConvertTo(typeof(bool), "y", false), true);
            TestAssert.AreEqual(Utils.ConvertTo(typeof(bool), "Yes", false), true);
            TestAssert.AreEqual(Utils.ConvertTo(typeof(bool), "yes", false), true);
            TestAssert.AreEqual(Utils.ConvertTo(typeof(TestEnum), 0, TestEnum.Enum1), TestEnum.Enum1);
        }

        public static void ToRmb()
        {
            TestAssert.AreEqual(Utils.ToRmb(12.45M), "壹拾贰元肆角伍分");
            TestAssert.AreEqual(Utils.ToRmb(12M), "壹拾贰元整");
        }

        public static void FromBase64String()
        {
            var expect = "test";
            var value = Utils.ToBase64String(expect);
            var actual = Utils.FromBase64String(value);
            TestAssert.AreEqual(actual, expect);
        }

        public static void ToBase64String()
        {
            var value = Utils.ToBase64String("test");
            var value1 = Utils.ToBase64String("test");
            TestAssert.AreEqual(value, value1);
        }

        public static void HideMobile()
        {
            TestAssert.AreEqual(Utils.HideMobile("13812345678"), "138****5678");
        }

        public static void Round()
        {
            TestAssert.AreEqual(Utils.Round(null, 2), null);
            TestAssert.AreEqual(Utils.Round(0.124M, 2).Value, 0.12M);
            TestAssert.AreEqual(Utils.Round(0.125M, 2).Value, 0.13M);
            TestAssert.AreEqual(Utils.Round(0.126M, 2).Value, 0.13M);
        }

        public static void GetUniqueString()
        {
            TestAssert.AreEqual(Utils.GetUniqueString(), "12345678");
            TestAssert.AreEqual(Utils.GetUniqueString(6), "123456");
            TestAssert.AreEqual(Utils.GetUniqueString(10), "1234567890");
        }

        public static void EnsureFile()
        {
            var path = string.Format("{0}\\test", Environment.CurrentDirectory);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            TestAssert.AreEqual(Directory.Exists(path), false);

            var fileName = string.Format("{0}\\test.txt", path);
            Utils.EnsureFile(fileName);
            TestAssert.AreEqual(Directory.Exists(path), true);
        }

        public static void DeleteFile()
        {
            var fileName = string.Format("{0}\\test\\test.txt", Environment.CurrentDirectory);
            File.AppendAllText(fileName, "test");
            TestAssert.AreEqual(File.Exists(fileName), true);

            Utils.DeleteFile(fileName);
            TestAssert.AreEqual(File.Exists(fileName), false);
        }

        public static void GetFileExtName()
        {
            var fileName = string.Format("{0}\\test\\test.txt", Environment.CurrentDirectory);
            TestAssert.AreEqual(Utils.GetFileExtName(fileName), ".txt");

            fileName = string.Format("{0}\\test\\test.log", Environment.CurrentDirectory);
            TestAssert.AreEqual(Utils.GetFileExtName(fileName), ".log");
        }
    }
}
