using Castle.DynamicProxy;

namespace Sample.Client;

public class HttpClientInterceptor<T>(IServiceProvider provider) : HttpInterceptor<T>(provider), IAsyncInterceptor where T : class
{
    private IHttpClientFactory HttpClientFactory => ServiceProvider.GetRequiredService<IHttpClientFactory>();

    protected override HttpClient CreateClient()
    {
        var type = typeof(T);
        return HttpClientFactory.CreateClient(type.Name);
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