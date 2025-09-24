using Sample.Tests;

namespace Sample;

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
        Config.RenderMode = RenderType.Auto;

        var assembly = typeof(AppConfig).Assembly;
        services.AddKnown(info =>
        {
            info.Id = AppId;
            info.Name = AppName;
            info.Assembly = assembly;
        });
        services.AddModules();
        services.ConfigUI();
    }

    // 添加客户端
    internal static void AddApplicationClient(this IServiceCollection services, Action<ClientOption> action)
    {
        services.AddKnownAdminClient();
        services.AddKnownClient(action);
    }

    private static void AddModules(this IServiceCollection services)
    {
        Config.Modules.AddItem("0", AppConstant.Demo, "示例页面", "block", 2);
    }

    private static void ConfigUI(this IServiceCollection services)
    {
        UIConfig.UserFormShowFooter = true;
        UIConfig.UserFormTabs.Set<UserDataForm>(2, "数据权限");
    }
}