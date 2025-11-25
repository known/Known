using Known.Sample;
using Known.Services;

namespace Known.Web;

class TestWorker(INotifyService service) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //if (++count % 10 == 0)
            //    service.LayoutNotifyAsync("系统通知", "TestWorker运行中......");

            await service.SendAsync(AppConstant.AddLog, new ConsoleLogInfo
            {
                BizId = "Test",
                Type = ConsoleLogType.Info,
                Content = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 测试日志信息"
            }, stoppingToken);
            await Task.Delay(_interval, stoppingToken);
        }
    }
}