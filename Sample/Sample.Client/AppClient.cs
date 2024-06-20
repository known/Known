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
            info.InterceptorType = type => typeof(HttpClientInterceptor<>).MakeGenericType(type);
            info.InterceptorProvider = (type, interceptor) =>
            {
                return Generator.CreateInterfaceProxyWithoutTarget(type, ((IAsyncInterceptor)interceptor).ToInterceptor());
            };
        });
        services.AddSampleRazor();
    }
}