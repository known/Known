using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Known;

public static class Extension
{
    public static void AddKnown(this IServiceCollection services)
    {
        var assembly = typeof(Config).Assembly;
        Config.AddModule(assembly);

        //services.AddCascadingAuthenticationState();

        if (!Config.IsWeb)
            services.AddScoped<IScrollToLocationHash, ScrollToLocationHash>();
        services.AddScoped<UIService>();
        services.AddScoped<ProtectedSessionStorage>();
        services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

        services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddIdentityCookies(b =>
                {
                    b.ApplicationCookie?.Configure(o =>
                    {
                        o.LoginPath = "/login";
                    });
                });

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