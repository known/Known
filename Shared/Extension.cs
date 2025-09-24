namespace Known;

/// <summary>
/// 框架配置扩展类。
/// </summary>
public static partial class Extension
{
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

    private static void AddKnownInnerCore(this IServiceCollection services, Action<AppInfo> action = null)
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

        services.AddScoped<ImportContext>();

        action?.Invoke(Config.App);
        CoreConfig.StartTime = DateTime.Now;
        Logger.Initialize(Config.App.WebLogDays);
        WeixinApi.Initialize(Config.App.Weixin);
        AppHelper.LoadConnections();
        LoadBuildTime(Config.Version);

        if (Config.App.Database != null)
            services.AddKnownData(Config.App.Database);
    }

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