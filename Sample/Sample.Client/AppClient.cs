namespace Sample.Client;

public static class AppClient
{
    public static void AddSampleRazor(this IServiceCollection services)
    {
        services.AddKnownAntDesign(option =>
        {
            //option.Footer = b => b.Component<Foot>().Build();
        });

        Config.AddModule(typeof(AppClient).Assembly);
    }

    internal static void AddSampleClient(this IServiceCollection services)
    {
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        services.AddSingleton<IAuthStateProvider, ClientAuthStateProvider>();
        services.AddSingleton<AuthenticationStateProvider, PersistentStateProvider>();
        services.AddKnownClient();

        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IApplyService, ApplyService>();
    }
}