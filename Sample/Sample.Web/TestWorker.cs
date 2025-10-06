using Known;
using Known.Extensions;
using Known.Services;

namespace Sample;

class TestWorker(INotifyService service) : BackgroundService
{
    //private int count = 0;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            while (true)
            {
                //if (++count % 10 == 0)
                //    service.LayoutNotifyAsync("系统通知", "TestWorker运行中......");

                service.SendAsync(AppConstant.AddLog, new ConsoleLogInfo
                {
                    BizId = "Test",
                    Type = ConsoleLogType.Info,
                    Content = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 测试日志信息"
                }, stoppingToken);
                Thread.Sleep(1000);
            }
        });
    }
}