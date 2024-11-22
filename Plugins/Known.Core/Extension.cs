using Microsoft.AspNetCore.Identity;

namespace Known.Core;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    private static readonly CoreOption option = new();

    /// <summary>
    /// 添加Known框架后端配置。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">系统配置方法。</param>
    public static void AddKnownCore(this IServiceCollection services, Action<AppInfo> action = null)
    {
        action?.Invoke(Config.App);
        if (Config.App.Type == AppType.WebApi)
            return;

        LoadBuildTime(Config.Version);
    }

    /// <summary>
    /// 添加桌面框架及身份认证支持。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">配置委托。</param>
    public static void AddKnownWin(this IServiceCollection services, Action<CoreOption> action = null)
    {
        AppHelper.LoadConnections();
        action?.Invoke(option);
        if (option.IsCompression)
            services.AddResponseCompression();
        services.AddHttpContextAccessor();
        services.AddCascadingAuthenticationState();
        services.AddScoped<IAuthStateProvider, WinAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider, WinAuthStateProvider>();
    }

    /// <summary>
    /// 添加Web框架及身份认证支持。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">配置委托。</param>
    public static void AddKnownWeb(this IServiceCollection services, Action<CoreOption> action = null)
    {
        AppHelper.LoadConnections();
        action?.Invoke(option);
        if (option.IsCompression)
            services.AddResponseCompression();
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddCascadingAuthenticationState();
        switch (option.AuthMode)
        {
            case AuthMode.Cookie:
                services.AddAuthentication().AddCookie(Constant.KeyAuth);
                services.AddScoped<IAuthStateProvider, CookieAuthStateProvider>();
                break;
            case AuthMode.Session:
                services.AddScoped<IAuthStateProvider, SessionAuthStateProvider>();
                break;
            case AuthMode.Identity:
                services.AddScoped<AuthenticationStateProvider, IdentityAuthStateProvider>();
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                }).AddIdentityCookies();
                services.AddIdentityCore<UserInfo>().AddSignInManager().AddDefaultTokenProviders();
                services.AddScoped<IUserStore<UserInfo>, UserStore>();
                services.AddScoped<IAuthStateProvider>(sp => (IdentityAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 使用框架静态文件和WebApi。
    /// </summary>
    /// <param name="app">Web应用程序。</param>
    public static void UseKnown(this WebApplication app)
    {
        if (option.IsCompression)
            app.UseResponseCompression();

        app.UseStaticFiles();
        var webFiles = Config.GetUploadPath(true);
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(webFiles),
            RequestPath = "/Files"
        });
        var upload = Config.GetUploadPath();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(upload),
            RequestPath = "/UploadFiles"
        });

        if (option.IsAddWebApi)
            app.UseKnownWebApi();

        app.MapControllers();
        Config.ServiceProvider = app.Services;
    }

    private static void UseKnownWebApi(this IEndpointRouteBuilder app)
    {
        foreach (var item in Config.ApiMethods)
        {
            if (item.HttpMethod == HttpMethod.Get)
                app.MapGet(item.Route, ctx => WebApi.Invoke(ctx, item));
            else
                app.MapPost(item.Route, ctx => WebApi.Invoke(ctx, item));
        }
    }

    private static void LoadBuildTime(VersionInfo info)
    {
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