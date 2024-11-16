using Coravel;
using Coravel.Invocable;

namespace Sample.Web;

public static class AppWeb
{
    public static void AddApplication(this WebApplicationBuilder builder)
    {
        Config.IsDevelopment = builder.Configuration.GetSection("IsDevelopment").Get<bool>();
        //Stopwatcher.Enabled = true;
        builder.Services.AddSampleCore();
        builder.Services.AddKnownCore(info =>
        {
            //info.ProductId = "Test";
            //info.CheckSystem = info => Result.Error("无效密钥，请重新授权！");
            info.WebRoot = builder.Environment.WebRootPath;
            info.ContentRoot = builder.Environment.ContentRootPath;
        });
        builder.Services.AddKnownData(option =>
        {
            var connString = builder.Configuration.GetSection("ConnString").Get<string>();
            option.AddProvider<Microsoft.Data.Sqlite.SqliteFactory>("Default", DatabaseType.SQLite, connString);
            //option.AddProvider<System.Data.OleDb.OleDbFactory>("Default", DatabaseType.Access, connString);
            //option.AddProvider<System.Data.SqlClient.SqlClientFactory>("Default", DatabaseType.SqlServer, connString);
            //option.AddProvider<MySqlConnector.MySqlConnectorFactory>("Default", DatabaseType.MySql, connString);
            //option.AddProvider<Npgsql.NpgsqlFactory>("Default", DatabaseType.PgSql, connString);
            //option.AddProvider<Dm.DmClientFactory>("Default", DatabaseType.DM, connString);
            //option.SqlMonitor = c => Console.WriteLine($"{DateTime.Now:HH:mm:ss} {c}");
        });
        builder.Services.AddKnownCells();
        builder.Services.AddKnownWeb(option =>
        {
            // 设置登录认证方式，默认Session
            //option.AuthMode = AuthMode.Cookie;
        });

        builder.Services.AddScheduler();
        builder.Services.AddTransient<ImportTaskJob>();
    }

    public static void UseApplication(this WebApplication app)
    {
        //使用Known框架
        app.UseKnown();
        //配置定时任务
        app.Services.UseScheduler(scheduler =>
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