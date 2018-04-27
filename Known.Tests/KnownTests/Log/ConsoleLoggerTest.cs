﻿using Known.Log;

namespace Known.Tests.KnownTests.Log
{
    public class ConsoleLoggerTest
    {
        public static void TestConsoleLogger()
        {
            var logger = new ConsoleLogger();
            for (int i = 0; i < 3; i++)
            {
                logger.Trace("测试Trace{0}", i);
            }
            logger.Info("测试结束");
            logger.Info("这是Trace信息：{0}", logger.TraceInfo);
            logger.Error("发生未知错误");
        }
    }
}
