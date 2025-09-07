namespace Sample.WinForm;

public static class AppConfig
{
    public static string AppId => "KIMS";
    public static string AppName => "Known信息管理系统";

    public static void AddApplication(this IServiceCollection services)
    {
        Console.WriteLine(AppName);
#if DEBUG
        Config.IsDevelopment = true;
        Config.IsDebug = true;
#endif

        var assembly = typeof(AppConfig).Assembly;
        services.AddKnown(info =>
        {
            info.Id = AppId;
            info.Name = AppName;
            info.Assembly = assembly;
            info.IsModule = true;
        });
        services.AddModules();
        services.AddServices(assembly);
        services.AddKnownWin(option =>
        {
            option.App.WebRoot = AppContext.BaseDirectory;
            option.App.ContentRoot = AppContext.BaseDirectory;
            option.Database = db =>
            {
                db.AddSQLite<Microsoft.Data.Sqlite.SqliteFactory>(@"Data Source=.\Sample.db");
                //db.SqlMonitor = c => Console.WriteLine($"{DateTime.Now:HH:mm:ss} {c}");
                //db.OperateMonitors.Add(info => Console.WriteLine(info.ToString()));
            };
        });
    }

    private static void AddModules(this IServiceCollection services)
    {
        Config.Modules.AddItem("0", AppConstant.Demo, "示例页面", "block", 2);
    }
}