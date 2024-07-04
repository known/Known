using Coravel;
using Coravel.Invocable;

namespace Sample.Web;

public static class AppWeb
{
    public static void AddApp(this WebApplicationBuilder builder, Action<AppInfo> action = null)
    {
        //Stopwatcher.Enabled = true;
        builder.Services.AddSample();
        builder.Services.AddSampleCore(info =>
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
                //DatabaseType = DatabaseType.MySql,
                //ProviderType = typeof(MySqlConnector.MySqlConnectorFactory),
                //DatabaseType = DatabaseType.Npgsql,
                //ProviderType = typeof(Npgsql.NpgsqlFactory),
                //DatabaseType = DatabaseType.SqlServer,
                //ProviderType = typeof(System.Data.SqlClient.SqlClientFactory),
                ConnectionString = builder.Configuration.GetSection("ConnString").Get<string>()
            }];
        });
        builder.Services.AddSampleRazor();

        builder.Services.AddKnownCells();
        builder.Services.AddKnownWeb();
        builder.Services.AddKnownWebApi();

        builder.Services.AddScheduler();
        builder.Services.AddTransient<ImportTaskJob>();
    }

    public static void UseApp(this WebApplication app)
    {
        //使用Known框架
        app.UseKnown();

        //配置认证
        //app.UseAuthentication();
        //app.UseAuthorization();

        //配置定时任务
        app.Services.UseScheduler(scheduler =>
        {
            //每5秒执行一次异步导入
            scheduler.Schedule<ImportTaskJob>().EveryFiveSeconds();
        });
    }

    private static void AddSampleCore(this IServiceCollection services, Action<AppInfo> action = null)
    {
        services.AddKnownCore(info =>
        {
            //info.ProductId = "Test";
            //info.CheckSystem = info => Result.Error("无效密钥，请重新授权！");
            action?.Invoke(info);
        });

        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IApplyService, ApplyService>();

        //注册待办事项显示流程表单
        //Config.ShowMyFlow = flow =>
        //{
        //    if (flow.Flow.FlowCode == AppFlow.Apply.Code)
        //        ApplyForm.ShowMyFlow(flow);
        //};
    }
}

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.ExecuteAsync();
}