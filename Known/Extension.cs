namespace Known;

public static class Extension
{
    public static void AddKnown(this IServiceCollection services, Action<AppInfo> action = null)
    {
        Config.StartTime = DateTime.Now;
        Language.Initialize();
        action?.Invoke(Config.App);

        services.AddScoped<JSService>();
        services.AddScoped<Context>();
        services.AddScoped<PlatformService>();
        services.AddSingleton<ICodeGenerator, CodeGenerator>();

        var content = Utils.GetResource(typeof(Extension).Assembly, "IconFA");
        if (!string.IsNullOrWhiteSpace(content))
        {
            var lines = content.Split([.. Environment.NewLine]);
            UIConfig.Icons["FontAwesome"] = lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => $"fa fa-{l}").ToList();
        }
    }

    public static void AddKnownCore(this IServiceCollection services, Action<AppInfo> action = null)
    {
        action?.Invoke(Config.App);

        if (Config.App.Connections != null && Config.App.Connections.Count > 0)
        {
            Database.RegisterConnections(Config.App.Connections);
            Database.Initialize();
        }
        Config.AddApp();
    }

    public static void AddKnownWebApi(this IServiceCollection services)
    {
        foreach (var type in Config.ServiceTypes.Values)
        {
            if (type.IsInterface || !type.GetInterfaces().Contains(typeof(IService)))
                continue;

            var controler = type.Name.Replace("Service", "");
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (method.IsPublic && method.DeclaringType?.Name == type.Name)
                {
                    var name = method.Name.Replace("Async", "");
                    var pattern = $"/{controler}/{name}";
                    Config.ApiMethods[pattern] = method;
                }
            }
        }
    }
}