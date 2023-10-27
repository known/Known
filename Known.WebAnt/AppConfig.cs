using Coravel;
using Coravel.Invocable;
using Known.Helpers;

namespace Known.WebAnt;

static class AppConfig
{
    internal static void InitApp(this WebApplicationBuilder builder)
    {
        //设置根目录
        Config.WebRoot = builder.Environment.WebRootPath;
        Config.ContentRoot = builder.Environment.ContentRootPath;

        //设置项目ID、名称和版本
        Config.AppId = "KIMS";
        Config.AppName = "Known信息管理系统";
        Config.SetAppVersion(typeof(AppConfig).Assembly);

        //设置产品ID，根据硬件获取ID
        Config.ProductId = $"{Config.AppId}-000001";

        //设置项目JS路径，通过UI.InvokeAppVoidAsync调用JS方法
        Config.AppJsPath = "/script.js";
        
        //设置默认分页大小
        PagingCriteria.DefaultPageSize = 20;

        //获取配置
        var configuration = builder.Configuration;
        var connString = configuration.GetSection("ConnString").Get<string>();
        var uploadPath = configuration.GetSection("UploadPath").Get<string>();

        //注册数据库访问提供者
        Database.RegisterProviders(new Dictionary<string, Type>
        {
            ["SQLite"] = typeof(Microsoft.Data.Sqlite.SqliteFactory)
            //["Npgsql"] = typeof(Npgsql.NpgsqlFactory)
            //["MySql"] = typeof(MySqlConnector.MySqlConnectorFactory)
            //["Access"] = typeof(System.Data.OleDb.OleDbFactory)
            //["SqlClient"] = typeof(System.Data.SqlClient.SqlClientFactory)
        });
        //配置数据库连接
        var connInfo = new Known.ConnectionInfo
        {
            Name = "Default",
            ProviderName = "SQLite",
            ConnectionString = connString
        };
        Config.App = new AppInfo
        {
            Connections = [connInfo],
            UploadPath = uploadPath
        };
    }

    internal static void AddApp(this IServiceCollection services)
    {
        //添加定时任务
        services.AddScheduler();
        services.AddTransient<ImportTaskJob>();
    }

    internal static void UseApp(this IServiceProvider provider)
    {
        //配置定时任务
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