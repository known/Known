using Known.Sample;

namespace Known.Wasm;

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
        //Config.RenderMode = RenderType.Auto;
        Config.AddModule(typeof(AppConfig).Assembly);

        services.AddKnown(info =>
        {
            info.Id = AppId;
            info.Name = AppName;
        });
        services.AddSample();
        services.ConfigUI();
    }

    // 添加客户端
    internal static void AddApplicationClient(this IServiceCollection services, Action<ClientOption> action)
    {
        services.AddApplication();
        services.AddKnownClient(action);
    }

    private static void ConfigUI(this IServiceCollection services)
    {
        KStyleSheet.AddStyle("css/app.css");
    }
}