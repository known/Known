using AntDesign;

namespace Known;

/// <summary>
/// 框架配置扩展类。
/// </summary>
public static class Extension
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

        Config.AddApp();
        services.AddAntDesign();

        services.AddSingleton<IEncryptService, EncryptService>();
        services.AddScoped<Context>();
        services.AddScoped<UIContext>();
        services.AddScoped<UIService>();
        services.AddScoped<JSService>();
        if (Config.App.IsClient)
            services.AddScoped<IAuthStateProvider, JSAuthStateProvider>();
        else
            services.AddScoped<IAuthStateProvider, AuthStateProvider>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IAutoService, AutoService>();
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
        services.AddScoped<IPlatformService, PlatformClient>();
        services.AddScoped<IAdminService, AdminClient>();
        services.AddScoped<IAutoService, AutoClient>();
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

        //AddInterceptors(services, option);
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

        action?.Invoke(Config.App);
        if (Config.App.Type == AppType.WebApi)
            return;

        AppData.LoadAppData();
        AppData.LoadBizData();
        LoadBuildTime(Config.Version);
    }

    /// <summary>
    /// 添加Known框架简易ORM数据访问组件。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">ORM配置选项委托。</param>
    public static void AddKnownData(this IServiceCollection services, Action<DatabaseOption> action = null)
    {
        action?.Invoke(DatabaseOption.Instance);
        services.AddScoped<Database>();
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
        KStyleSheet.AddStyle("_content/Known/css/web.css");
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
            new ActionInfo { Id = "Default", Style = "size", Url = "_content/Known/css/size/default.css" },
            new ActionInfo { Id = "Compact", Style = "size", Url = "_content/Known/css/size/compact.css" }
        ];

        var routes = "/install,/login,/profile,/profile/user,/profile/password,/app,/app/mine";
        UIConfig.IgnoreRoutes.AddRange(routes.Split(','));
        //用户中心
        UIConfig.UserProfileType = typeof(UserProfileInfo);
        UIConfig.UserTabs.Set<UserEditForm>(1, "MyProfile");
        UIConfig.UserTabs.Set<PasswordEditForm>(2, "SecuritySetting");
        //企业信息
        UIConfig.CompanyTabs.Set<CompanyBaseInfo>(1, "BasicInfo");
        //关于系统
        UIConfig.SystemTabs.Set<SysSystemInfo>(1, "SystemInfo");
        UIConfig.SystemTabs.Set<SysSystemSetting>(2, "SystemSetting");
    }

    //private static void AddInterceptors(IServiceCollection services, ClientOption option)
    //{
    //    if (option.InterceptorType == null)
    //        return;

    //    foreach (var type in Config.ApiTypes)
    //    {
    //        //Console.WriteLine(type.Name);
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