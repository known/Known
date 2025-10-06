using Known;
using Known.Services;

namespace Sample;

class TestWorker(INotifyService service) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            while (true)
            {
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