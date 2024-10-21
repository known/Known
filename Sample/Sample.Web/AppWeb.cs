using Coravel;
using Coravel.Invocable;

namespace Sample.Web;

public static class AppWeb
{
    public static void AddApp(this IServiceCollection services, Action<AppInfo> action = null)
    {
        var assembly = typeof(AppWeb).Assembly;
        ModuleHelper.InitAppModules();
        //Stopwatcher.Enabled = true;
        services.AddSample();
        services.AddKnownCore(info =>
        {
            //info.ProductId = "Test";
            //info.CheckSystem = info => Result.Error("无效密钥，请重新授权！");
            //info.SqlMonitor = c => Console.WriteLine($"{DateTime.Now:HH:mm:ss} {c}");
            action?.Invoke(info);
        });
        services.AddKnownCells();
        switch (Config.App.Type)
        {
            case AppType.Web:
                services.AddKnownWeb(option => option.AddAssembly(assembly));
                break;
            case AppType.Desktop:
                services.AddKnownWin(option => option.AddAssembly(assembly));
                break;
        }

        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IApplyService, ApplyService>();

        //注册待办事项显示流程表单
        //Config.ShowMyFlow = flow =>
        //{
        //    if (flow.Flow.FlowCode == AppFlow.Apply.Code)
        //        ApplyForm.ShowMyFlow(flow);
        //};

        services.AddScheduler();
        services.AddTransient<ImportTaskJob>();
    }

    public static void UseApp(this WebApplication app)
    {
        //使用Known框架
        app.UseKnown();
        //配置定时任务
        app.Services.UseApp();
    }

    public static void UseApp(this IServiceProvider provider)
    {
        provider.UseScheduler(scheduler =>
        {
            //每5秒执行一次异步导入
            scheduler.Schedule<ImportTaskJob>().EveryFiveSeconds();
        });
    }
}

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.ExecuteAsync();
}