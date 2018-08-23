using System;
using System.IO;

namespace Known.Tests.KnownTests
{
    public class UtilsTest
    {
        public static void TestNewGuid()
        {
            Assert.IsEqual(Utils.NewGuid.Length, 32);
        }

        public static void TestIsNullOrEmpty()
        {
            Assert.IsEqual(Utils.IsNullOrEmpty(null), true);
            Assert.IsEqual(Utils.IsNullOrEmpty(DBNull.Value), true);
            Assert.IsEqual(Utils.IsNullOrEmpty(""), true);
            Assert.IsEqual(Utils.IsNullOrEmpty(" "), true);
            Assert.IsEqual(Utils.IsNullOrEmpty("test"), false);
        }

        public static void TestConvertTo1()
        {
            Assert.IsEqual(Utils.ConvertTo<int>("1"), 1);
            Assert.IsEqual(Utils.ConvertTo<decimal>("1.2"), 1.2M);
            Assert.IsEqual(Utils.ConvertTo<bool>(1), true);
            Assert.IsEqual(Utils.ConvertTo<bool>(0), false);
            Assert.IsEqual(Utils.ConvertTo<bool>("Y"), true);
            Assert.IsEqual(Utils.ConvertTo<bool>("y"), true);
            Assert.IsEqual(Utils.ConvertTo<bool>("Yes"), true);
            Assert.IsEqual(Utils.ConvertTo<bool>("yes"), true);
            Assert.IsEqual(Utils.ConvertTo<TestEnum>(0), TestEnum.Enum1);
        }

        public static void TestConvertTo2()
        {
            Assert.IsEqual(Utils.ConvertTo(typeof(int), "1", 0), 1);
            Assert.IsEqual(Utils.ConvertTo(typeof(decimal), "1.2", 0), 1.2M);
            Assert.IsEqual(Utils.ConvertTo(typeof(bool), 1, false), true);
            Assert.IsEqual(Utils.ConvertTo(typeof(bool), 0, false), false);
            Assert.IsEqual(Utils.ConvertTo(typeof(bool), "Y", false), true);
            Assert.IsEqual(Utils.ConvertTo(typeof(bool), "y", false), true);
            Assert.IsEqual(Utils.ConvertTo(typeof(bool), "Yes", false), true);
            Assert.IsEqual(Utils.ConvertTo(typeof(bool), "yes", false), true);
            Assert.IsEqual(Utils.ConvertTo(typeof(TestEnum), 0, TestEnum.Enum1), TestEnum.Enum1);
        }

        public static void TestToRmb()
        {
            Assert.IsEqual(Utils.ToRmb(12.45M), "壹拾贰元肆角伍分");
            Assert.IsEqual(Utils.ToRmb(12M), "壹拾贰元整");
        }

        public static void TestFromBase64String()
        {
            var expect = "test";
            var value = Utils.ToBase64String(expect);
            var actual = Utils.FromBase64String(value);
            Assert.IsEqual(actual, expect);
        }

        public static void TestToBase64String()
        {
            var value = Utils.ToBase64String("test");
            var value1 = Utils.ToBase64String("test");
            Assert.IsEqual(value, value1);
        }

        public static void TestHideMobile()
        {
            Assert.IsEqual(Utils.HideMobile("13812345678"), "138****5678");
        }

        public static void TestRound()
        {
            Assert.IsEqual(Utils.Round(null, 2), null);
            Assert.IsEqual(Utils.Round(0.124M, 2).Value, 0.12M);
            Assert.IsEqual(Utils.Round(0.125M, 2).Value, 0.13M);
            Assert.IsEqual(Utils.Round(0.126M, 2).Value, 0.13M);
        }
        public static void TestGetUniqueString()
        {
            Assert.IsEqual(Utils.GetUniqueString(), "12345678");
            Assert.IsEqual(Utils.GetUniqueString(6), "123456");
            Assert.IsEqual(Utils.GetUniqueString(10), "1234567890");
        }

        public static void TestEnsureFile()
        {
            var path = string.Format("{0}\\test", Environment.CurrentDirectory);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Assert.IsEqual(Directory.Exists(path), false);

            var fileName = string.Format("{0}\\test.txt", path);
            Utils.EnsureFile(fileName);
            Assert.IsEqual(Directory.Exists(path), true);
        }

        public static void TestDeleteFile()
        {
            var fileName = string.Format("{0}\\test\\test.txt", Environment.CurrentDirectory);
            File.AppendAllText(fileName, "test");
            Assert.IsEqual(File.Exists(fileName), true);

            Utils.DeleteFile(fileName);
            Assert.IsEqual(File.Exists(fileName), false);
        }

        public static void TestGetFileExtName()
        {
            var fileName = string.Format("{0}\\test\\test.txt", Environment.CurrentDirectory);
            Assert.IsEqual(Utils.GetFileExtName(fileName), ".txt");

            fileName = string.Format("{0}\\test\\test.log", Environment.CurrentDirectory);
            Assert.IsEqual(Utils.GetFileExtName(fileName), ".log");
        }
    }
}
