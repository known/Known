using Coravel;
using Coravel.Invocable;

namespace Sample.Web;

public static class AppWeb
{
    public static void AddApplication(this IServiceCollection services, Action<AppInfo> action)
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
                services.AddKnownWeb(option =>
                {
                    // 设置登录认证方式，默认Session
                    //option.AuthMode = AuthMode.Cookie;
                    option.AddAssembly(assembly);
                });
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

    public static void AddApplication(this WebApplicationBuilder builder)
    {
        Config.IsDevelopment = builder.Configuration.GetSection("IsDevelopment").Get<bool>();
        builder.Services.AddApplication(info =>
        {
            info.WebRoot = builder.Environment.WebRootPath;
            info.ContentRoot = builder.Environment.ContentRootPath;
            //数据库连接
            info.Connections = [new Known.ConnectionInfo
            {
                Name = "Default",
                DatabaseType = DatabaseType.SQLite,
                ProviderType = typeof(Microsoft.Data.Sqlite.SqliteFactory),
                //DatabaseType = DatabaseType.Access,
                //ProviderType = typeof(System.Data.OleDb.OleDbFactory),
                //DatabaseType = DatabaseType.SqlServer,
                //ProviderType = typeof(System.Data.SqlClient.SqlClientFactory),
                //DatabaseType = DatabaseType.MySql,
                //ProviderType = typeof(MySqlConnector.MySqlConnectorFactory),
                //DatabaseType = DatabaseType.PgSql,
                //ProviderType = typeof(Npgsql.NpgsqlFactory),
                //DatabaseType = DatabaseType.DM,
                //ProviderType = typeof(Dm.DmClientFactory),
                ConnectionString = builder.Configuration.GetSection("ConnString").Get<string>()
            }];
        });
    }

    public static void UseApplication(this WebApplication app)
    {
        //使用Known框架
        app.UseKnown();
        //配置定时任务
        app.Services.UseApplication();
    }

    public static void UseApplication(this IServiceProvider provider)
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