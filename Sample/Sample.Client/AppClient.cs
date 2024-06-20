using Castle.DynamicProxy;

namespace Sample.Client;

public static class AppClient
{
    private static readonly ProxyGenerator Generator = new();

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
        services.AddKnownClient(info =>
        {
            info.BaseUrl = "http://localhost";
            info.Provider = (provider, type) =>
            {
                var interceptorType = typeof(HttpClientInterceptor<>).MakeGenericType(type);
                services.AddTransient(interceptorType);
                return Generator.CreateInterfaceProxyWithoutTarget(type, ((IAsyncInterceptor)provider.GetRequiredService(interceptorType)).ToInterceptor());
            };
        });
        services.AddSampleRazor();
    }
}