using Known.Blazor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
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

        //services.AddCascadingAuthenticationState();

        //if (Config.App.Type == AppType.WinForm)
        //    services.AddScoped<IScrollToLocationHash, ScrollToLocationHash>();
        services.AddScoped<JSService>();
        //services.AddScoped<ProtectedSessionStorage>();
        services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
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

class ScrollToLocationHash : IScrollToLocationHash
{
    public Task RefreshScrollPositionForHash(string locationAbsolute)
    {
        return Task.CompletedTask;
    }
}