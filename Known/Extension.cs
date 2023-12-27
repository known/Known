using Known.Blazor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Known;

public static class Extension
{
    public static void AddKnown(this IServiceCollection services, Action<AppInfo> action = null)
    {
        action?.Invoke(Config.App);
        Config.AddApp();

        if (Config.App.IsDevelopment)
            Logger.Level = LogLevel.Debug;
        else
            Logger.Level = LogLevel.Info;

        //services.AddCascadingAuthenticationState();
        services.AddScoped<JSService>();
        services.AddScoped<ICodeService, CodeService>();
        if (Config.App.Type == AppType.Web)
        {
            services.AddScoped<ProtectedSessionStorage>();
            services.AddScoped<AuthenticationStateProvider, WebAuthStateProvider>();
        }
        else if (Config.App.Type == AppType.WinForm)
        {
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, WinAuthStateProvider>();
        }
        services.AddHttpContextAccessor();
        //services.AddOptions().AddLogging();
    }

    public static void UseKnownStaticFiles(this IApplicationBuilder app)
    {
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
    }
}