using Known.Razor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Known;

public static class Extension
{
    public static void AddKnown(this IServiceCollection services)
    {
        var assembly = typeof(Extension).Assembly;
        Config.AddModule(assembly);
        ActionInfo.Load();

        //services.AddCascadingAuthenticationState();

        if (Config.App.Type == AppType.WinForm)
            services.AddScoped<IScrollToLocationHash, ScrollToLocationHash>();
        services.AddScoped<JSService>();
        services.AddScoped<ProtectedSessionStorage>();
        services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
        services.AddHttpContextAccessor();
        //services.AddOptions().AddLogging();
    }
}

class ScrollToLocationHash : IScrollToLocationHash
{
    public Task RefreshScrollPositionForHash(string locationAbsolute)
    {
        return Task.CompletedTask;
    }
}