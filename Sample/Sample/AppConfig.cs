namespace Sample;

public static class AppConfig
{
    private static readonly List<MenuInfo> AppMenus =
    [
        new MenuInfo { Id = "Home", Name = "首页", Icon = "home", Target = "Tab", Url = "/app" },
        new MenuInfo { Id = "Mine", Name = "我的", Icon = "user", Target = "Tab", Url = "/app/mine" },
        new MenuInfo { Id = "Test", Name = "测试模块", Icon = "form", Target = "Menu", Color = "#1890ff", Url = "/app/test" },
        new MenuInfo { Id = "Add", Name = "功能待加", Icon = "plus", Target = "Menu", Color = "#4fa624" }
    ];

    public const string Branch = "Known";
    public const string SubTitle = "基于Blazor的企业级快速开发框架";

    public static string AppName { get; set; }

    public static void AddSample(this IServiceCollection services, Action<AppInfo> action = null)
    {
        Console.WriteLine(AppName);
        Config.AppMenus = AppMenus;

        services.AddKnown(info =>
        {
            //项目ID、名称、类型、程序集
            info.Id = "KIMS";
            info.Name = AppName;
            info.IsLanguage = true;
            info.IsTheme = true;
            //JS路径，通过JS.InvokeAppVoidAsync调用JS方法
            info.JsPath = "./script.js";
            action?.Invoke(info);
        });

        //添加模块
        Config.AddModule(typeof(AppConfig).Assembly);
    }
}