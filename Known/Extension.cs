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
        Language.Initialize();
        action?.Invoke(Config.App);

        if (Config.App.IsDevelopment)
            Logger.Level = LogLevel.Debug;
        else
            Logger.Level = LogLevel.Info;

        Database.RegisterProviders(Config.App.Connections);
        Database.Initialize();
        Config.AddApp();

        //services.AddCascadingAuthenticationState();
        services.AddScoped<JSService>();
        services.AddScoped<ICodeGenerator, CodeGenerator>();
        if (Config.App.Type == AppType.Web)
        {
            services.AddScoped<ProtectedSessionStorage>();
            services.AddScoped<AuthenticationStateProvider, WebAuthStateProvider>();
        }
        else if (Config.App.Type == AppType.Desktop)
        {
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, WinAuthStateProvider>();
        }
        services.AddHttpContextAccessor();
        //services.AddOptions().AddLogging();

        var content = Utils.GetResource(typeof(Extension).Assembly, "IconFA");
        if (!string.IsNullOrWhiteSpace(content))
        {
            var lines = content.Split([.. Environment.NewLine]);
            UIConfig.Icons["FontAwesome"] = lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => $"fa fa-{l}").ToList();
        }
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