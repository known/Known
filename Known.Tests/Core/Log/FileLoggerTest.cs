using System;
using System.IO;
using Known.Log;

namespace Known.Tests.Core.Log
{
    public class FileLoggerTest
    {
        public static void FileLogger()
        {
            var fileName = string.Format("{0}\\test\\test.log", Environment.CurrentDirectory);
            Utils.DeleteFile(fileName);

            var logger = new FileLogger(fileName);
            for (int i = 0; i < 3; i++)
            {
                logger.Trace("测试Trace{0}", i);
            }
            logger.Info("测试结束");
            logger.Info("这是Trace信息：{0}", logger.TraceInfo);
            logger.Error("发生未知错误");

            var log = System.IO.File.ReadAllText(fileName);
            TestAssert.IsNotNull(log);
        }
    }
}
