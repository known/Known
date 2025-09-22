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

        action?.Invoke(Config.App);
        WeixinApi.Initialize(Config.App.Weixin);
        services.AddScoped<ImportContext>();
    }
}