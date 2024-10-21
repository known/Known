//using Castle.DynamicProxy;

//namespace Sample;

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
//    public Task SetUserAsync(UserInfo user) => js.SetUserInfoAsync(user);
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