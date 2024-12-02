using Coravel;
using Coravel.Invocable;
using Sample.Web.Services;

namespace Sample.Web;

public static class AppConfig
{
    private static readonly List<MenuInfo> AppMenus =
    [
        new MenuInfo { Id = "Home", Name = "首页", Icon = "home", Target = "Tab", Url = "/app" },
        new MenuInfo { Id = "Mine", Name = "我的", Icon = "user", Target = "Tab", Url = "/app/mine" },
        new MenuInfo { Id = "Test", Name = "测试模块", Icon = "form", Target = "Menu", Color = "#1890ff", Url = "/app/test" },
        new MenuInfo { Id = "Add", Name = "功能待加", Icon = "plus", Target = "Menu", Color = "#4fa624" }
    ];

    public static string AppId => "KIMS";
    public static string AppName => "Known信息管理系统";

    public static void AddApplication(this WebApplicationBuilder builder)
    {
        Console.WriteLine(AppName);
        Config.AppMenus = AppMenus;
        Config.IsDevelopment = builder.Configuration.GetSection("IsDevelopment").Get<bool>();
#if DEBUG
        Config.IsDebug = true;
#endif
        //Stopwatcher.Enabled = true;
        builder.Services.AddAppWeb();
        builder.Services.AddAppWebCore(builder.Configuration);
        builder.Services.AddKnownCore(info =>
        {
            info.WebRoot = builder.Environment.WebRootPath;
            info.ContentRoot = builder.Environment.ContentRootPath;
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

    private static void AddAppWeb(this IServiceCollection services)
    {
        var assembly = typeof(AppConfig).Assembly;
        services.AddKnown(info =>
        {
            info.Id = AppId;
            info.Name = AppName;
            info.Assembly = assembly;
            //JS路径，通过JS.InvokeAppVoidAsync调用JS方法
            //info.JsPath = "./script.js";
        });
        services.AddKnownAdmin();

        //UIConfig.AutoTablePage = (b, m) => b.Component<CustomTablePage>().Set(c => c.Model, m).Build();
        UIConfig.Errors["403"] = new ErrorConfigInfo { Description = "你没有此页面的访问权限。" };
    }

    private static void AddAppWebCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKnownData(option =>
        {
            var connString = configuration.GetSection("ConnString").Get<string>();
            //option.AddAccess<System.Data.OleDb.OleDbFactory>(connString);
            option.AddSQLite<Microsoft.Data.Sqlite.SqliteFactory>(connString);
            //option.AddSqlServer<Microsoft.Data.SqlClient.SqlClientFactory>(connString);
            //option.AddMySql<MySqlConnector.MySqlConnectorFactory>(connString);
            //option.AddPgSql<Npgsql.NpgsqlFactory>(connString);
            //option.AddDM<Dm.DmClientFactory>(connString);
            //option.SqlMonitor = c => Console.WriteLine($"{DateTime.Now:HH:mm:ss} {c}");
        });
        services.AddKnownAdminCore(option =>
        {
            //option.ProductId = "Test";
            //option.CheckSystem = info => Result.Error("无效密钥，请重新授权！");
            //option.AddModules(ModuleHelper.AddAppModules);
        });
        services.AddKnownCells();
        services.AddKnownWeb();

        // 注入服务
        services.AddScoped<IHomeService, HomeService>();
    }
}

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.ExecuteAsync();
}