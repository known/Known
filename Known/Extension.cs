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
        services.AddScoped<Context>();

        if (Config.App.Type == AppType.WebApi)
            return;

        Config.AddModule(typeof(Extension).Assembly);
        services.AddAntDesign();
        services.AddScoped<UIContext>();
        services.AddScoped<JSService>();
        services.AddScoped<UIService>();
        services.AddSingleton<ICodeGenerator, CodeGenerator>();
        services.AddSingleton<INotifyService, NotifyService>();
        services.AddScoped<IAuthStateProvider, AuthStateProvider>();
        services.AddScoped<IPluginService, PluginService>();

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
        services.AddKnownCore(action);
    }

    /// <summary>
    /// 添加Known框架WebApi应用配置。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">系统配置方法。</param>
    public static void AddKnownWebApi(this IServiceCollection services, Action<AppInfo> action = null)
    {
        services.AddKnownInnerCore(action);
    }

    /// <summary>
    /// 添加Known框架后端配置。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">系统配置方法。</param>
    public static void AddKnownCore(this IServiceCollection services, Action<AppInfo> action = null)
    {
        services.AddKnownInnerCore(action);

        services.AddScoped(typeof(IEntityService<>), typeof(EntityService<>));
        services.AddScoped<ImportContext>();
        services.AddScoped<IUserHandler, UserHandler>();

        Config.AddAppCore();
        CoreConfig.StartTime = DateTime.Now;
        CoreConfig.OnRoleModule = (db, id) => db.GetRoleModuleIdsAsync(id);
        Logger.Initialize(Config.App.WebLogDays);
        WeixinApi.Initialize(Config.App.Weixin);
        AppHelper.LoadConnections();
        LoadBuildTime(Config.Version);

        // 添加模型
        DbConfig.Models.Add<SysRoleModule>(x => new { x.RoleId, x.ModuleId });
        DbConfig.Models.Add<SysUserRole>(x => new { x.UserId, x.RoleId });

        services.LoadServers();
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
        KScript.AddScript("_content/Known/js/libs/zxing.js");
        KScript.AddScript("_content/Known/js/libs/prism.js");
        if (Config.IsNotifyHub)
            KScript.AddScript("_content/Known/js/libs/signalr.js");
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
        UIConfig.Errors["403"] = new ErrorConfigInfo { Description = "无权限访问！" };
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
        //企业信息
        UIConfig.CompanyTabs.Set<CompanyBaseInfo>(1, Language.BasicInfo);

        // 添加一级模块
        if (Config.App.IsAddMenu)
        {
            Config.Modules.AddItem("0", Constants.BaseData, Language.BaseData, "database", 1);
            Config.Modules.AddItem("0", Constants.System, Language.SystemManage, "setting", 99);
        }
    }

    private static void AddKnownInnerCore(this IServiceCollection services, Action<AppInfo> action)
    {
        action?.Invoke(Config.App);

        if (string.IsNullOrWhiteSpace(Config.App.WebRoot))
            Config.App.WebRoot = AppDomain.CurrentDomain.BaseDirectory;
        if (string.IsNullOrWhiteSpace(Config.App.ContentRoot))
            Config.App.ContentRoot = AppDomain.CurrentDomain.BaseDirectory;

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            var content = e.Exception.ToString();
            if (!content.Contains("JSDisconnectedException"))
            {
                Logger.Error(LogTarget.Task, new UserInfo { Name = sender.ToString() }, content);
                Logger.Exception("TASK", e.Exception);
            }
            e.SetObserved(); // 标记为已处理
        };
        // 进程级，无法阻止程序退出
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                Logger.Exception("DOMAIN", ex);
                Config.App.OnExit?.Invoke(ex);
            }
        };

        if (Config.App.Database != null)
            services.AddKnownData(Config.App.Database);
    }

    private static void LoadBuildTime(VersionInfo info)
    {
        if (info == null)
            return;

        var dateTime = GetBuildTime();
        var count = (int)(dateTime - new DateTime(2020, 7, 10)).TotalDays;
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