namespace Known;

public static class Extension
{
    public static void AddKnown(this IServiceCollection services, Action<AppInfo> action = null)
    {
        Config.StartTime = DateTime.Now;
        Logger.Level = LogLevel.Info;
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

        UIConfig.FillHeightScript = @"
var page = $('.kui-page').outerHeight(true) || 0;
var query = $('.kui-page .kui-query').outerHeight(true) || 0;
var tabs = $('.kui-page .ant-tabs-nav').outerHeight(true) || 0;
var toolbar = $('.kui-table .kui-toolbar').outerHeight(true) || 0;
var tableHead = $('.kui-table .ant-table-header').outerHeight(true) || 0;
var pagination = $('.kui-table .ant-table-pagination').outerHeight(true) || 0;
//console.log('page='+page+',query='+query+',tabs='+tabs+',toolbar='+toolbar+',tableHead='+tableHead+',pagination='+pagination);
var cardHeight = page-tabs-10;
var tableHeight = page-query-tabs-toolbar-tableHead-pagination-20;
$('.kui-card .ant-tabs-content-holder').css('height', cardHeight+'px');
$('.kui-table .ant-table-body').not('.form-list .ant-table-body').css('height', tableHeight+'px');";
    }

    public static void AddKnownCore(this IServiceCollection services, Action<AppInfo> action = null)
    {
        Config.Version.LoadBuildTime();
        FileLogger.Start();
        Config.CoreAssemblies.Add(typeof(Extension).Assembly);
        action?.Invoke(Config.App);

        DBHelper.RegisterConnections();
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