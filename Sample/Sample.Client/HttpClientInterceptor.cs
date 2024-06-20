using Castle.DynamicProxy;

namespace Sample.Client;

public class HttpClientInterceptor<T>(IServiceScopeFactory provider) : HttpInterceptor<T>(provider), IAsyncInterceptor where T : class
{
    protected override async Task<HttpClient> CreateClientAsync()
    {
        var type = typeof(T);
        var factory = await ServiceFactory.CreateAsync<IHttpClientFactory>();
        var client = factory.CreateClient(type.Name);
        client.BaseAddress = new Uri(Config.BaseUrl);
        return client;
    }

    public void InterceptAsynchronous(IInvocation invocation)
    {
        invocation.ReturnValue = SendAsync(invocation.Method, invocation.Arguments);
    }

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        invocation.ReturnValue = SendAsync<TResult>(invocation.Method, invocation.Arguments);
    }

    public async void InterceptSynchronous(IInvocation invocation)
    {
        invocation.ReturnValue = await SendAsync(invocation.Method, invocation.Arguments);
    }
}