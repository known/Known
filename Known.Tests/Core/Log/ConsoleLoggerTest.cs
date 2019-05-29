using Known.Log;

namespace Known.Tests.Core.Log
{
    public class ConsoleLoggerTest
    {
        public static void ConsoleLogger()
        {
            var logger = new ConsoleLogger();
            for (int i = 0; i < 3; i++)
            {
                logger.Trace($"测试Trace{i}");
            }
            logger.Info("测试结束");
            logger.Info($"这是Trace信息：{logger.TraceInfo}");
            logger.Error("发生未知错误");
        }
    }
}
