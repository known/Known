namespace Known;

public static class Extension
{
    public static void AddKnown(this IServiceCollection services, Action<AppInfo> action = null)
    {
        Config.StartTime = DateTime.Now;
        Language.Initialize();
        action?.Invoke(Config.App);

        if (Config.App.IsDevelopment)
            Logger.Level = LogLevel.Debug;
        else
            Logger.Level = LogLevel.Info;

        if (Config.App.Connections != null && Config.App.Connections.Count > 0)
        {
            Database.RegisterConnections(Config.App.Connections);
            Database.Initialize();
        }
        Config.AddApp();

        //services.AddCascadingAuthenticationState();
        services.AddScoped<JSService>();
        services.AddScoped<ICodeGenerator, CodeGenerator>();
        services.AddScoped<Context>();
        services.AddScoped<PlatformService>();
        //services.AddOptions().AddLogging();

        var content = Utils.GetResource(typeof(Extension).Assembly, "IconFA");
        if (!string.IsNullOrWhiteSpace(content))
        {
            var lines = content.Split([.. Environment.NewLine]);
            UIConfig.Icons["FontAwesome"] = lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => $"fa fa-{l}").ToList();
        }
    }
}