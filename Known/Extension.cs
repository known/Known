namespace Known;

public static class Extension
{
    public static void AddKnown(this IServiceCollection services, Action<AppInfo> action = null)
    {
        Config.StartTime = DateTime.Now;
        Language.Initialize();
        action?.Invoke(Config.App);

        services.AddScoped<Context>();
        services.AddScoped<JSService>();
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

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAutoService, AutoService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IDictionaryService, DictionaryService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IFlowService, FlowService>();
        services.AddScoped<ISystemService, SystemService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWeixinService, WeixinService>();
    }
}