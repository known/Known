namespace Sample.WinForm;

public static class AppConfig
{
    public const string AppId = "KIMS";
    public const string AppName = "Known信息管理系统";

    public static void AddApplication(this IServiceCollection services)
    {
        services.AddKnown(info =>
        {
            info.Id = AppId;
            info.Name = AppName;
            info.Type = AppType.Desktop;
            info.WebRoot = Application.StartupPath;
            info.ContentRoot = Application.StartupPath;
            info.Assembly = typeof(AppConfig).Assembly;
        });
    }
}