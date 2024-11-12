//using System.Security.Claims;
//using Castle.DynamicProxy;
//using Microsoft.AspNetCore.Components.Authorization;

//namespace Sample;

//static class AppClient
//{
//    private static readonly ProxyGenerator Generator = new();

//    internal static void AddSampleClient(this IServiceCollection services)
//    {
//        services.AddHttpClient();
//        //services.AddAuthorizationCore();
//        //services.AddCascadingAuthenticationState();
//        services.AddScoped<IAuthStateProvider, AuthStateProvider>();
//        //services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
//        services.AddSample();
//        services.AddKnownClient(info =>
//        {
//            info.InterceptorType = type => typeof(HttpClientInterceptor<>).MakeGenericType(type);
//            info.InterceptorProvider = (type, interceptor) =>
//            {
//                return Generator.CreateInterfaceProxyWithoutTarget(type, ((IAsyncInterceptor)interceptor).ToInterceptor());
//            };
//        });
//    }
//}

//class AuthStateProvider : AuthenticationStateProvider, IAuthStateProvider
//{
//    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
//        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

//    private readonly JSService js;
//    private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

//    public AuthStateProvider(JSService js, PersistentComponentState state)
//    {
//        this.js = js;
//        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
//            return;

//        Claim[] claims = [
//            new Claim(ClaimTypes.NameIdentifier, userInfo.UserName),
//            new Claim(ClaimTypes.Name, userInfo.Email),
//            new Claim(ClaimTypes.Email, userInfo.Email) ];

//        authenticationStateTask = Task.FromResult(
//            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
//                authenticationType: nameof(AuthStateProvider)))));
//    }

//    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;

//    public Task<UserInfo> GetUserAsync() => js.GetUserInfoAsync();
//    public Task SignInAsync(UserInfo user) => js.SetUserInfoAsync(user);
//    public Task SignOutAsync() => js.SetUserInfoAsync(null);
//}

//class HttpClientInterceptor<T>(IServiceScopeFactory provider) : HttpInterceptor<T>(provider), IAsyncInterceptor where T : class
//{
//    protected override async Task<HttpClient> CreateClientAsync()
//    {
//        var type = typeof(T);
//        var factory = await ServiceFactory.CreateAsync<IHttpClientFactory>();
//        var client = factory.CreateClient(type.Name);
//        client.BaseAddress = new Uri(Config.HostUrl);
//        return client;
//    }

//    public void InterceptAsynchronous(IInvocation invocation)
//    {
//        invocation.ReturnValue = SendAsync(invocation.Method, invocation.Arguments);
//    }

//    public void InterceptAsynchronous<TResult>(IInvocation invocation)
//    {
//        invocation.ReturnValue = SendAsync<TResult>(invocation.Method, invocation.Arguments);
//    }

//    public void InterceptSynchronous(IInvocation invocation)
//    {
//        //Console.WriteLine($"同步={invocation.Method}");
//        //invocation.ReturnValue = await SendAsync(invocation.Method, invocation.Arguments);
//    }
//}