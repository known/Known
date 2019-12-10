using System;
using System.IO;
using Known.Helpers;

namespace Known.Tests.Helpers
{
    public class FileHelperTest
    {
        public static void EnsureFile()
        {
            var path = string.Format("{0}\\test", Environment.CurrentDirectory);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            TestAssert.AreEqual(Directory.Exists(path), false);

            var fileName = string.Format("{0}\\test.txt", path);
            FileHelper.EnsureFile(fileName);
            TestAssert.AreEqual(Directory.Exists(path), true);
        }

        public static void DeleteFile()
        {
            var fileName = string.Format("{0}\\test\\test.txt", Environment.CurrentDirectory);
            File.AppendAllText(fileName, "test");
            TestAssert.AreEqual(File.Exists(fileName), true);

            FileHelper.DeleteFile(fileName);
            TestAssert.AreEqual(File.Exists(fileName), false);
        }

        public static void GetFileExtName()
        {
            var fileName = string.Format("{0}\\test\\test.txt", Environment.CurrentDirectory);
            TestAssert.AreEqual(FileHelper.GetFileExtName(fileName), ".txt");

            fileName = string.Format("{0}\\test\\test.log", Environment.CurrentDirectory);
            TestAssert.AreEqual(FileHelper.GetFileExtName(fileName), ".log");
        }
    }
}
