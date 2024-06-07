using Known.AntBlazor;

namespace Sample.Client;

public static class AppClient
{
    public static void AddClient(this IServiceCollection services)
    {
        Config.IsClient = true;

        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        services.AddSingleton<IAuthStateProvider, ClientAuthStateProvider>();
        services.AddSingleton<AuthenticationStateProvider, PersistentStateProvider>();

        //services.AddDemo();
        services.AddKnownAntDesign(option =>
        {
            //option.Footer = b => b.Component<Foot>().Build();
        });
    }
}