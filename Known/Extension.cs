using AntDesign;

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
        Config.StartTime = DateTime.Now;
        action?.Invoke(Config.App);

        if (Config.App.Type == AppType.WebApi)
            return;

        var assembly = typeof(Extension).Assembly;
        Config.AddApp(assembly);
        services.AddAntDesign();

        services.AddScoped<Context>();
        services.AddScoped<UIContext>();
        services.AddServices(assembly);
        if (Config.App.IsClient)
            services.AddScoped<IAuthStateProvider, JSAuthStateProvider>();
        else
            services.AddScoped<IAuthStateProvider, AuthStateProvider>();
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
        services.AddClients(typeof(Extension).Assembly);
        services.AddScoped(typeof(IEntityService<>), typeof(EntityClient<>));

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
        //services.AddInterceptors(option);
    }

    /// <summary>
    /// 添加Known框架后端配置。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">系统配置方法。</param>
    public static void AddKnownCore(this IServiceCollection services, Action<AppInfo> action = null)
    {
        if (string.IsNullOrWhiteSpace(Config.App.WebRoot))
            Config.App.WebRoot = AppDomain.CurrentDomain.BaseDirectory;
        if (string.IsNullOrWhiteSpace(Config.App.ContentRoot))
            Config.App.ContentRoot = AppDomain.CurrentDomain.BaseDirectory;

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            var content = e.Exception.ToString();
            if (!content.Contains("JSDisconnectedException"))
                Logger.Error(LogTarget.Task, new UserInfo { Name = sender.ToString() }, content);
            e.SetObserved(); // 标记为已处理
        };
        // 进程级，无法阻止程序退出
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                Logger.Exception(ex);
                Config.App.OnExit?.Invoke(ex);
            }
        };

        action?.Invoke(Config.App);
        if (Config.App.Type == AppType.WebApi)
            return;

        if (Config.IsAdmin)
        {
            UIConfig.TopNavType = null; // 移除Admin插件顶部导航
            UIConfig.EnableEdit = true;
        }

        AppData.KmdPath = Path.Combine(Config.App.ContentRoot, "AppData.kmd");
        AppData.KcdPath = Path.Combine(Config.App.ContentRoot, "AppData.kcd");
        AppData.KdbPath = Path.Combine(Config.App.ContentRoot, "AppData.kdb");
        // 设置当前路径为程序根目录（适配Maui）
        //Environment.CurrentDirectory = AppContext.BaseDirectory;
        AppData.LoadAppData();
        AppData.LoadBizData();
        LoadBuildTime(Config.Version);
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
        KStyleSheet.AddStyle("_content/Known/css/web.min.css");
    }

    private static void AddScripts()
    {
        KScript.AddScript("_content/AntDesign/js/ant-design-blazor.js");
        KScript.AddScript("_content/Known/js/libs/jquery.js");
        KScript.AddScript("_content/Known/js/libs/pdfobject.js");
        //KScript.AddScript("_content/Known/js/libs/highcharts.js");
        KScript.AddScript("_content/Known/js/libs/barcode.js");
        KScript.AddScript("_content/Known/js/libs/qrcode.js");
        KScript.AddScript("_content/Known/js/libs/prism.js");
        KScript.AddScript("_content/Known/js/web.js");
        KScript.AddScript("_content/Known/js/serviceWorkerRegister.js");
    }

    private static void ConfigureUI()
    {
        UIConfig.EnableEdit = true;
        UIConfig.Icons["AntDesign"] = typeof(IconType.Outline).GetProperties().Select(x => (string)x.GetValue(null)).Where(x => x is not null).ToList();
        var content = Utils.GetResource(typeof(Extension).Assembly, "IconFA");
        if (!string.IsNullOrWhiteSpace(content))
        {
            var lines = content.Split([.. Environment.NewLine]);
            UIConfig.Icons["FontAwesome"] = lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => $"fa fa-{l}").ToList();
        }
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
        //企业信息
        UIConfig.CompanyTabs.Set<CompanyBaseInfo>(1, Language.BasicInfo);
        //关于系统
        UIConfig.SystemTabs.Set<SysSystemInfo>(1, Language.SystemInfo);
        UIConfig.SystemTabs.Set<SysSystemSetting>(2, Language.SystemSetting);
    }

    //private static void AddInterceptors(this IServiceCollection services, ClientOption option)
    //{
    //    if (option.InterceptorType == null)
    //        return;

    //    foreach (var type in Config.ApiTypes)
    //    {
    //        var interceptorType = option.InterceptorType.Invoke(type);
    //        if (interceptorType == null)
    //            continue;

    //        services.AddScoped(interceptorType);
    //        services.AddScoped(type, provider =>
    //        {
    //            var interceptor = provider.GetRequiredService(interceptorType);
    //            return option.InterceptorProvider?.Invoke(type, interceptor);
    //        });
    //    }
    //}

    private static void LoadBuildTime(VersionInfo info)
    {
        if (info == null)
            return;

        var dateTime = GetBuildTime();
        var count = dateTime.Year - 2000 + dateTime.Month + dateTime.Day;
        info.BuildTime = dateTime;
        info.SoftVersion = $"{info.SoftVersion}.{count}";
    }

    private static DateTime GetBuildTime()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        var fileName = Directory.GetFiles(path, "*.exe")?.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(fileName))
        {
            //var version = assembly?.GetName().Version;
            //return new DateTime(2000, 1, 1) + TimeSpan.FromDays(version.Revision);
            //return new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            return DateTime.Now;
        }

        var file = new FileInfo(fileName);
        return file.LastWriteTime;
    }
}