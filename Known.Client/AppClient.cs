namespace Known.Client;

public static class AppClient
{
    public static void AddClient(this IServiceCollection services)
    {
        Config.IsClient = true;

        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        services.AddScoped<IAuthStateProvider, ClientAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider, PersistentStateProvider>();

        services.AddDemo();
        services.AddKnownAntDesign(option =>
        {
            //option.Footer = b => b.Component<Foot>().Build();
        });
    }
}