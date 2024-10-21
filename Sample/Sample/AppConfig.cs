//using Castle.DynamicProxy;

namespace Sample;

public static class AppConfig
{
    //private static readonly ProxyGenerator Generator = new();
    private static readonly List<MenuInfo> AppMenus =
    [
        new MenuInfo { Id = "Home", Name = "首页", Icon = "home", Target = "Tab", Url = "/app" },
        new MenuInfo { Id = "Mine", Name = "我的", Icon = "user", Target = "Tab", Url = "/app/mine" },
        new MenuInfo { Id = "Test", Name = "测试模块", Icon = "form", Target = "Menu", Color = "#1890ff", Url = "/app/test" },
        new MenuInfo { Id = "Add", Name = "功能待加", Icon = "plus", Target = "Menu", Color = "#4fa624" }
    ];

    public const string Branch = "Known";
    public const string SubTitle = "基于Blazor的企业级快速开发框架";
    public static string AppId => "KIMS";
    public static string AppName => "Known信息管理系统";

    public static void AddSample(this IServiceCollection services)
    {
        Console.WriteLine(AppName);
        Config.AppMenus = AppMenus;
        //系统默认设置
        //Config.OnSetting = s =>
        //{
        //    s.MultiTab = true;    // 是否标签页
        //    s.Accordion = true;   // 是否手风琴
        //    s.Collapsed = true;   // 是否折叠
        //    s.MenuTheme = "Dark"; // Light/Dark两种
        //};
        services.AddKnown(info =>
        {
            //项目ID、名称、类型、程序集
            info.Id = AppId;
            info.Name = AppName;
            info.IsSize = true;
            info.IsLanguage = true;
            info.IsTheme = true;
            info.Assembly = typeof(AppConfig).Assembly;
            //JS路径，通过JS.InvokeAppVoidAsync调用JS方法
            info.JsPath = "./script.js";
        });
        services.AddKnownAntDesign(option =>
        {
            //option.Footer = b => b.Component<Foot>().Build();
        });

        //UIConfig.AutoTablePage = (b, m) => b.Component<CustomTablePage>().Set(c => c.Model, m).Build();
        UIConfig.Errors["403"] = new ErrorConfigInfo { Description = "你没有此页面的访问权限。" };

        //添加模块
        Config.AddModule(typeof(AppConfig).Assembly);
    }

    //internal static void AddSampleClient(this IServiceCollection services)
    //{
    //    services.AddHttpClient();
    //    services.AddAuthorizationCore();
    //    services.AddCascadingAuthenticationState();
    //    services.AddScoped<IAuthStateProvider, AuthStateProvider>();
    //    services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
    //    services.AddKnownClient(info =>
    //    {
    //        info.InterceptorType = type => typeof(HttpClientInterceptor<>).MakeGenericType(type);
    //        info.InterceptorProvider = (type, interceptor) =>
    //        {
    //            return Generator.CreateInterfaceProxyWithoutTarget(type, ((IAsyncInterceptor)interceptor).ToInterceptor());
    //        };
    //    });
    //    services.AddSampleRazor();
    //}
}