﻿using Sample.Web.Tests;

namespace Sample.Web;

public static class AppConfig
{
    private static readonly List<MenuInfo> AppMenus =
    [
        new MenuInfo { Id = "Home", Name = "首页", Icon = "home", Target = "Tab", Url = "/app" },
        new MenuInfo { Id = "App", Name = "应用", Icon = "appstore", Target = "Tab", Url = "/app/mine" },
        new MenuInfo { Id = "Disc", Name = "发现", Icon = "compass", Target = "Tab", Url = "/app/mine" },
        new MenuInfo { Id = "Mine", Name = "我的", Icon = "user", Target = "Tab", Url = "/app/mine" },
        new MenuInfo { Id = "Test", Name = "收货", Icon = "import", Target = "Menu", Color = "#1464ad", Url = "/app/test" },
        new MenuInfo { Id = "Test", Name = "上架", Icon = "vertical-align-top", Target = "Menu", Color = "#2db7f5", Url = "/app/test" },
        new MenuInfo { Id = "Test", Name = "下架", Icon = "vertical-align-bottom", Target = "Menu", Color = "#722ed1", Url = "/app/test" },
        new MenuInfo { Id = "Test", Name = "盘点", Icon = "schedule", Target = "Menu", Color = "#108ee9", Url = "/app/test" },
        new MenuInfo { Id = "Test", Name = "移库", Icon = "merge-cells", Target = "Menu", Color = "#f50", Url = "/app/test" },
        new MenuInfo { Id = "Test", Name = "库存", Icon = "insert-row-above", Target = "Menu", Color = "#fa8c16", Url = "/app/test" },
        new MenuInfo { Id = "Test", Name = "波次", Icon = "partition", Target = "Menu", Color = "#1890ff", Url = "/app/test" },
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

        builder.AddAppWeb();
        builder.AddAppWebCore();
    }

    public static void UseApplication(this WebApplication app)
    {
        app.UseKnown();
        app.UseTaskJobs();
    }

    private static void AddAppWeb(this WebApplicationBuilder builder)
    {
        var assembly = typeof(AppConfig).Assembly;
        builder.Services.AddKnown(info =>
        {
            info.Id = AppId;
            info.Name = AppName;
            info.Assembly = assembly;
            info.IsMobile = true;
            //info.NextIdType = NextIdType.AutoInteger;
        });
        builder.Services.AddKnownAdmin();
        builder.Services.AddModules();
        builder.Services.ConfigUI();
    }

    private static void AddAppWebCore(this WebApplicationBuilder builder)
    {
        var assembly = typeof(AppConfig).Assembly;
        builder.Services.AddServices(assembly);
        builder.Services.AddKnownAdminCore(option =>
        {
            //option.Code = new CodeConfigInfo
            //{
            //    EntityPath = @"D:\Sample",
            //    PagePath = @"D:\Sample.Client",
            //    ServicePath = @"D:\Sample.Web"
            //};
            //option.ProductId = "Test";
            //option.CheckSystem = info => Result.Error("无效密钥，请重新授权！");
            //option.AddModules(ModuleHelper.AddAppModules);
        });
        builder.Services.AddKnownCells();
        builder.Services.AddKnownWeb(option =>
        {
            option.App.WebRoot = builder.Environment.WebRootPath;
            option.App.ContentRoot = builder.Environment.ContentRootPath;
            option.Database = db =>
            {
                var connString = builder.Configuration.GetSection("ConnString").Get<string>();
                //db.AddAccess<System.Data.OleDb.OleDbFactory>(connString);
                db.AddSQLite<Microsoft.Data.Sqlite.SqliteFactory>(connString);
                //db.AddSqlServer<Microsoft.Data.SqlClient.SqlClientFactory>(connString);
                //db.AddSqlServer<Oracle.ManagedDataAccess.Client.OracleClientFactory>(connString);
                //db.AddMySql<MySqlConnector.MySqlConnectorFactory>(connString);
                //db.AddPgSql<Npgsql.NpgsqlFactory>(connString);
                //db.AddDM<Dm.DmClientFactory>(connString);
                //db.SqlMonitor = c => Console.WriteLine($"{DateTime.Now:HH:mm:ss} {c}");
                //db.OperateMonitors.Add(info => Console.WriteLine(info.ToString()));
            };
        });

        // 添加任务
        builder.Services.AddTaskJobs();
    }

    private static void AddModules(this IServiceCollection services)
    {
        Config.Modules.Add(AppConstant.Demo, "示例页面", "block", "0", 2);
    }

    private static void ConfigUI(this IServiceCollection services)
    {
        UIConfig.EnableEdit = true;
        UIConfig.UserFormTabs.Set<UserDataForm>(2, "数据权限");
    }
}