using System;
using System.IO;

namespace Known.Tests.Core
{
    public class UtilsTest
    {
        public static void TestNewGuid()
        {
            Assert.AreEqual(Utils.NewGuid.Length, 32);
        }

        public static void TestIsNullOrEmpty()
        {
            Assert.AreEqual(Utils.IsNullOrEmpty(null), true);
            Assert.AreEqual(Utils.IsNullOrEmpty(DBNull.Value), true);
            Assert.AreEqual(Utils.IsNullOrEmpty(""), true);
            Assert.AreEqual(Utils.IsNullOrEmpty(" "), true);
            Assert.AreEqual(Utils.IsNullOrEmpty("test"), false);
        }

        public static void TestConvertTo1()
        {
            Assert.AreEqual(Utils.ConvertTo<int>("1"), 1);
            Assert.AreEqual(Utils.ConvertTo<decimal>("1.2"), 1.2M);
            Assert.AreEqual(Utils.ConvertTo<bool>(1), true);
            Assert.AreEqual(Utils.ConvertTo<bool>(0), false);
            Assert.AreEqual(Utils.ConvertTo<bool>("Y"), true);
            Assert.AreEqual(Utils.ConvertTo<bool>("y"), true);
            Assert.AreEqual(Utils.ConvertTo<bool>("Yes"), true);
            Assert.AreEqual(Utils.ConvertTo<bool>("yes"), true);
            Assert.AreEqual(Utils.ConvertTo<TestEnum>(0), TestEnum.Enum1);
        }

        public static void TestConvertTo2()
        {
            Assert.AreEqual(Utils.ConvertTo(typeof(int), "1", 0), 1);
            Assert.AreEqual(Utils.ConvertTo(typeof(decimal), "1.2", 0), 1.2M);
            Assert.AreEqual(Utils.ConvertTo(typeof(bool), 1, false), true);
            Assert.AreEqual(Utils.ConvertTo(typeof(bool), 0, false), false);
            Assert.AreEqual(Utils.ConvertTo(typeof(bool), "Y", false), true);
            Assert.AreEqual(Utils.ConvertTo(typeof(bool), "y", false), true);
            Assert.AreEqual(Utils.ConvertTo(typeof(bool), "Yes", false), true);
            Assert.AreEqual(Utils.ConvertTo(typeof(bool), "yes", false), true);
            Assert.AreEqual(Utils.ConvertTo(typeof(TestEnum), 0, TestEnum.Enum1), TestEnum.Enum1);
        }

        public static void TestToRmb()
        {
            Assert.AreEqual(Utils.ToRmb(12.45M), "壹拾贰元肆角伍分");
            Assert.AreEqual(Utils.ToRmb(12M), "壹拾贰元整");
        }

        public static void TestFromBase64String()
        {
            var expect = "test";
            var value = Utils.ToBase64String(expect);
            var actual = Utils.FromBase64String(value);
            Assert.AreEqual(actual, expect);
        }

        public static void TestToBase64String()
        {
            var value = Utils.ToBase64String("test");
            var value1 = Utils.ToBase64String("test");
            Assert.AreEqual(value, value1);
        }

        public static void TestHideMobile()
        {
            Assert.AreEqual(Utils.HideMobile("13812345678"), "138****5678");
        }

        public static void TestRound()
        {
            Assert.AreEqual(Utils.Round(null, 2), null);
            Assert.AreEqual(Utils.Round(0.124M, 2).Value, 0.12M);
            Assert.AreEqual(Utils.Round(0.125M, 2).Value, 0.13M);
            Assert.AreEqual(Utils.Round(0.126M, 2).Value, 0.13M);
        }

        public static void TestGetUniqueString()
        {
            Assert.AreEqual(Utils.GetUniqueString(), "12345678");
            Assert.AreEqual(Utils.GetUniqueString(6), "123456");
            Assert.AreEqual(Utils.GetUniqueString(10), "1234567890");
        }

        public static void TestEnsureFile()
        {
            var path = string.Format("{0}\\test", Environment.CurrentDirectory);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Assert.AreEqual(Directory.Exists(path), false);

            var fileName = string.Format("{0}\\test.txt", path);
            Utils.EnsureFile(fileName);
            Assert.AreEqual(Directory.Exists(path), true);
        }

        public static void TestDeleteFile()
        {
            var fileName = string.Format("{0}\\test\\test.txt", Environment.CurrentDirectory);
            File.AppendAllText(fileName, "test");
            Assert.AreEqual(File.Exists(fileName), true);

            Utils.DeleteFile(fileName);
            Assert.AreEqual(File.Exists(fileName), false);
        }

        public static void TestGetFileExtName()
        {
            var fileName = string.Format("{0}\\test\\test.txt", Environment.CurrentDirectory);
            Assert.AreEqual(Utils.GetFileExtName(fileName), ".txt");

            fileName = string.Format("{0}\\test\\test.log", Environment.CurrentDirectory);
            Assert.AreEqual(Utils.GetFileExtName(fileName), ".log");
        }
    }
}
