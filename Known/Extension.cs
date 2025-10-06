namespace Known;

/// <summary>
/// 框架配置扩展类。
/// </summary>
public static partial class Extension
{
    /// <summary>
    /// 添加Known框架配置。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">系统配置方法。</param>
    public static void AddKnown(this IServiceCollection services, Action<AppInfo> action = null)
    {
        action?.Invoke(Config.App);

        if (Config.App.Type == AppType.WebApi)
            return;

        Config.AddApp();
        services.AddAntDesign();

        services.AddScoped<Context>();
        services.AddScoped<UIContext>();
        services.AddScoped<JSService>();
        services.AddScoped<UIService>();
        services.AddSingleton<ICodeGenerator, CodeGenerator>();
        services.AddSingleton<INotifyService, NotifyService>();

        if (Config.App.IsClient)
            services.AddScoped<IAuthStateProvider, JSAuthStateProvider>();
        else
            services.AddScoped<IAuthStateProvider, AuthStateProvider>();
        services.AddScoped<IUserPage, UserPage>();
        services.AddScoped<IPluginService, PluginService>();
        services.AddScoped(typeof(IEntityService<>), typeof(EntityService<>));

        AddStyles();
        AddScripts();
        ConfigureUI();
    }

    /// <summary>
    /// 添加Known框架客户端配置。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">客户端配置选项委托。</param>
    public static void AddKnownClient(this IServiceCollection services, Action<ClientOption> action = null)
    {
        Config.IsClient = true;
        action?.Invoke(ClientOption.Instance);

        services.AddScoped<IAuthStateProvider, JSAuthStateProvider>();
        services.AddScoped(typeof(IEntityService<>), typeof(EntityClient<>));
        services.LoadClients();

        var option = ClientOption.Instance;
        if (!string.IsNullOrWhiteSpace(option.BaseAddress))
        {
            services.AddSingleton(sp =>
            {
                var navi = sp.GetRequiredService<NavigationManager>();
                var handler = new AuthHttpHandler(navi);
                return new HttpClient(handler) { BaseAddress = new Uri(option.BaseAddress) };
            });
        }
    }

    /// <summary>
    /// 添加桌面框架及身份认证支持。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">配置委托。</param>
    public static void AddKnownDesktop(this IServiceCollection services, Action<AppInfo> action = null)
    {
        Config.IsNotifyHub = false;
        Config.App.Type = AppType.Desktop;
        //services.AddHttpContextAccessor();
        //services.AddCascadingAuthenticationState();
        services.AddScoped<IAuthStateProvider, AuthStateProvider>();
        //services.AddScoped<AuthenticationStateProvider, WinAuthStateProvider>();
        services.AddKnownCore(action);
    }

    /// <summary>
    /// 添加Known框架后端配置。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">系统配置方法。</param>
    public static void AddKnownCore(this IServiceCollection services, Action<AppInfo> action = null)
    {
        services.AddScoped(typeof(IEntityService<>), typeof(EntityService<>));
        services.AddKnownInnerCore(action);
        services.LoadServers();
    }

    /// <summary>
    /// 添加Known框架简易微信功能模块前端。
    /// </summary>
    /// <param name="services">服务集合。</param>
    public static void AddKnownWeixin(this IServiceCollection services)
    {
        // 配置UI
        UIConfig.SystemTabs.Set<WeChatSetting>(3, "WeChatSetting");
    }

    private static void AddStyles()
    {
        KStyleSheet.AddStyle("_content/AntDesign/css/ant-design-blazor.css");
        KStyleSheet.AddStyle("_content/Known/css/theme/default.css");
        KStyleSheet.AddStyle("_content/Known/css/size/default.css");
        KStyleSheet.AddStyle("_content/Known/css/font-awesome.css");
        KStyleSheet.AddStyle("_content/Known/css/prism.css");
        KStyleSheet.AddStyle("_content/Known/css/web.css");
    }

    private static void AddScripts()
    {
        KScript.AddScript("_content/AntDesign/js/ant-design-blazor.js");
        KScript.AddScript("_content/Known/js/libs/jquery.js");
        KScript.AddScript("_content/Known/js/libs/echarts.js");
        KScript.AddScript("_content/Known/js/libs/echarts-liquidfill.js");
        KScript.AddScript("_content/Known/js/libs/pdfobject.js");
        KScript.AddScript("_content/Known/js/libs/barcode.js");
        KScript.AddScript("_content/Known/js/libs/qrcode.js");
        KScript.AddScript("_content/Known/js/libs/prism.js");
        if (Config.IsNotifyHub)
            KScript.AddScript("_content/Known/js/libs/signalr.js");
        KScript.AddScript("_content/Known/js/libs/zxing.js");
        KScript.AddScript("_content/Known/js/web.js");
        KScript.AddScript("_content/Known/js/serviceWorkerRegister.js");
    }

    private static void ConfigureUI()
    {
        UIConfig.EnableEdit = true;
        IconHelper.LoadAntIcon();
        IconHelper.LoadFAIcon();
        UIConfig.Sizes = [
            new ActionInfo { Id = "Default", Name = Language.SizeDefault, Style = "size", Url = "_content/Known/css/size/default.css" },
            new ActionInfo { Id = "Compact", Name = Language.SizeCompact, Style = "size", Url = "_content/Known/css/size/compact.css" }
        ];

        var routes = "/install,/login,/profile,/profile/user,/profile/password,/app,/app/mine";
        UIConfig.IgnoreRoutes.AddRange(routes.Split(','));
        //模块管理
        UIConfig.ModuleTabs.Set<SysModuleList>(1, Language.SysModule);
        //用户中心
        UIConfig.UserProfileType = typeof(UserProfileInfo);
        UIConfig.UserTabs.Set<UserEditForm>(1, Language.MyProfile);
        UIConfig.UserTabs.Set<PasswordEditForm>(2, Language.SecuritySetting);
        //关于系统
        UIConfig.SystemTabs.Set<SysSystemInfo>(1, Language.SystemInfo);
        UIConfig.SystemTabs.Set<SecuritySetting>(2, Language.SecuritySetting);

        Config.Modules.AddItem("0", Constants.System, Language.SystemManage, "setting", 99);
    }
}