namespace Known;

/// <summary>
/// 框架配置扩展类。
/// </summary>
public static class Extension
{
    /// <summary>
    /// 添加框架配置。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">系统配置方法。</param>
    public static void AddKnown(this IServiceCollection services, Action<AppInfo> action = null)
    {
        Config.StartTime = DateTime.Now;
        Logger.Level = LogLevel.Info;
        action?.Invoke(Config.App);

        if (Config.App.Type == AppType.WebApi)
            return;

        Config.AddApp();
        services.AddScoped<Context>();
        services.AddScoped<JSService>();
        services.AddSingleton<ICodeGenerator, CodeGenerator>();

        var routes = "/,/install,/login,/profile,/profile/user,/profile/password,/app,/app/mine";
        UIConfig.IgnoreRoutes.AddRange(routes.Split(','));

        var content = Utils.GetResource(typeof(Extension).Assembly, "IconFA");
        if (!string.IsNullOrWhiteSpace(content))
        {
            var lines = content.Split([.. Environment.NewLine]);
            UIConfig.Icons["FontAwesome"] = lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => $"fa fa-{l}").ToList();
        }
    }

    

    /// <summary>
    /// 添加框架客户端配置。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">客户端配置方法。</param>
    public static void AddKnownClient(this IServiceCollection services, Action<ClientInfo> action = null)
    {
        Config.IsClient = true;
        var info = new ClientInfo();
        action?.Invoke(info);

        foreach (var type in Config.ApiTypes)
        {
            //Console.WriteLine(type.Name);
            var interceptorType = info.InterceptorType?.Invoke(type);
            services.AddScoped(interceptorType);
            services.AddScoped(type, provider =>
            {
                var interceptor = provider.GetRequiredService(interceptorType);
                return info.InterceptorProvider?.Invoke(type, interceptor);
            });
        }
    }
}