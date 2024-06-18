using Coravel;
using Coravel.Invocable;

namespace Sample.WinForm;

public static class AppWinForm
{
    public static void AddApp(this IServiceCollection services)
    {
        AppConfig.AppName = "Known信息管理系统";
        services.AddSample();
        services.AddSampleCore(info =>
        {
            info.WebRoot = Application.StartupPath;
            info.ContentRoot = Application.StartupPath;
            info.Assembly = typeof(AppWinForm).Assembly;

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
                ConnectionString = "Data Source=..\\Sample.db"
            }];
            //info.Connections[0].ConnectionString = "Data Source=..\\Sample.db";
            //info.Connections[0].ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Sample;Jet OLEDB:Database Password={password}";
            //info.Connections[0].ConnectionString = "Data Source=localhost;port=3306;Initial Catalog=Sample;user id={userId};password={password};Charset=utf8;SslMode=none;AllowZeroDateTime=True;";
            //info.Connections[0].ConnectionString = "Data Source=localhost;Initial Catalog=Sample;User Id={userId};Password={password};";
            //info.Connections[0].ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Sample;Trusted_Connection=True";
        });
        services.AddSampleRazor();

        services.AddKnownCells();
        services.AddKnownWin();

        services.AddScheduler();
        services.AddTransient<ImportTaskJob>();
    }

    public static void UseApp(this WebApplication app)
    {
        //使用Known框架静态文件
        app.UseKnownStaticFiles();

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
}

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.ExecuteAsync();
}