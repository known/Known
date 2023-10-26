using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Known;

public static class Extension
{
    public static void AddKnown(this IServiceCollection services)
    {
        Config.Modules.Add(typeof(Config).Assembly);

        services.AddRazorComponents()
                .AddInteractiveServerComponents();
        //services.AddCascadingAuthenticationState();

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