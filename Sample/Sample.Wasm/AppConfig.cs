namespace Sample.Wasm;

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
        Config.Modules.Add(AppConstant.Demo, "示例页面", "block", "0", 2);

        var assembly = typeof(AppConfig).Assembly;
        services.AddKnown(info =>
        {
            info.Id = AppId;
            info.Name = AppName;
            info.Assembly = assembly;
            info.IsClient = true;
        });
        services.AddServices(assembly);
        services.AddKnownCore();
    }
}