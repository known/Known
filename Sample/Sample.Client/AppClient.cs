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

        UIConfig.Errors["403"] = new ErrorConfigInfo { Description = "你没有此页面的访问权限。" };
    }

    internal static void AddSampleClient(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        services.AddScoped<IAuthStateProvider, AuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
        services.AddKnownClient(info =>
        {
            info.InterceptorType = type => typeof(HttpClientInterceptor<>).MakeGenericType(type);
            info.InterceptorProvider = (type, interceptor) =>
            {
                return Generator.CreateInterfaceProxyWithoutTarget(type, ((IAsyncInterceptor)interceptor).ToInterceptor());
            };
        });
        services.AddSampleRazor();
    }
}