using Microsoft.AspNetCore.Identity;

namespace Known.Core;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    private static readonly CoreOption option = new();

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
}