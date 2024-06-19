namespace Known;

public static class Extension
{
    public static void AddKnown(this IServiceCollection services, Action<AppInfo> action = null)
    {
        Config.StartTime = DateTime.Now;
        Language.Initialize();
        action?.Invoke(Config.App);
        Config.AddApp();

        services.AddScoped<Context>();
        services.AddScoped<JSService>();
        services.AddSingleton<ICodeGenerator, CodeGenerator>();

        var content = Utils.GetResource(typeof(Extension).Assembly, "IconFA");
        if (!string.IsNullOrWhiteSpace(content))
        {
            var lines = content.Split([.. Environment.NewLine]);
            UIConfig.Icons["FontAwesome"] = lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => $"fa fa-{l}").ToList();
        }
    }

    public static async void AddKnownCore(this IServiceCollection services, Action<AppInfo> action = null)
    {
        action?.Invoke(Config.App);

        if (Config.App.Connections != null && Config.App.Connections.Count > 0)
        {
            Database.RegisterConnections(Config.App.Connections);
            await Database.InitializeAsync();
        }

        var service = new SystemService(new Context());
        Config.Install = await service.GetInstallAsync();

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

    public static void AddKnownClient(this IServiceCollection services, Action<ClientInfo> action = null)
    {
        var info = new ClientInfo();
        action?.Invoke(info);

        services.AddScoped(http => new HttpClient
        {
            BaseAddress = new Uri(info.BaseUrl)
        });

        foreach (var type in Config.ApiTypes)
        {
            //Console.WriteLine(type.Name);
            services.AddScoped(type, provider => info.Provider?.Invoke(provider, type));
        }

        //services.AddScoped<IAuthService, AuthClient>();
        //services.AddScoped<IAutoService, AutoClient>();
        //services.AddScoped<ICompanyService, CompanyClient>();
        //services.AddScoped<IDictionaryService, DictionaryClient>();
        //services.AddScoped<IFileService, FileClient>();
        //services.AddScoped<IFlowService, FlowClient>();
        //services.AddScoped<ISystemService, SystemClient>();
        //services.AddScoped<ISettingService, SettingClient>();
        //services.AddScoped<IModuleService, ModuleClient>();
        //services.AddScoped<IRoleService, RoleClient>();
        //services.AddScoped<IUserService, UserClient>();
        //services.AddScoped<IWeixinService, WeixinClient>();
    }
}