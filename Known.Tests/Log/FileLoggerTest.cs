using System;
using System.IO;
using Known.Log;

namespace Known.Tests.Log
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
                logger.Trace($"测试Trace{i}");
            }
            logger.Info("测试结束");
            logger.Info($"这是Trace信息：{logger.TraceInfo}");
            logger.Error("发生未知错误");

            var log = File.ReadAllText(fileName);
            TestAssert.IsNotNull(log);
        }
    }
}
