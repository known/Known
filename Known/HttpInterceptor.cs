namespace Known;

/// <summary>
/// 抽象HTTP拦截器类。
/// </summary>
/// <typeparam name="T">拦截对象类型。</typeparam>
/// <param name="provider">依赖注入服务提供者。</param>
public abstract class HttpInterceptor<T>(IServiceScopeFactory provider) where T : class
{
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// 取得依赖注入服务工厂实例。
    /// </summary>
    protected IServiceScopeFactory ServiceFactory { get; } = provider;

    /// <summary>
    /// 创建HTTP客户端实例抽象方法。
    /// </summary>
    /// <returns></returns>
    protected abstract Task<HttpClient> CreateClientAsync();

    /// <summary>
    /// 发送HTTP请求。
    /// </summary>
    /// <param name="method">方法信息。</param>
    /// <param name="arguments">方法参数集合。</param>
    /// <returns>请求结果对象。</returns>
    protected async Task<object> SendAsync(MethodInfo method, object[] arguments)
    {
        var stream = await ReadStreamAsync(method, arguments);
        if (stream == null || stream.Length == 0)
            return default;

        if (method.ReturnType == typeof(void))
            return default;

        return await JsonSerializer.DeserializeAsync(stream, method.ReturnType, jsonOptions);
    }

    /// <summary>
    /// 发送HTTP请求。
    /// </summary>
    /// <typeparam name="TResult">结果泛型类型。</typeparam>
    /// <param name="method">方法信息。</param>
    /// <param name="arguments">方法参数集合。</param>
    /// <returns>请求结果泛型对象。</returns>
    protected async Task<TResult> SendAsync<TResult>(MethodInfo method, object[] arguments)
    {
        var stream = await ReadStreamAsync(method, arguments);
        if (stream == null || stream.Length == 0)
            return default;

        return await JsonSerializer.DeserializeAsync<TResult>(stream, jsonOptions);
    }

    private async Task<Stream> ReadStreamAsync(MethodInfo method, object[] arguments)
    {
        using var client = await CreateClientAsync();
        var request = await CreateRequestMessageAsync(method, arguments);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync();
    }

    private async Task<HttpRequestMessage> CreateRequestMessageAsync(MethodInfo method, object[] arguments)
    {
        var request = new HttpRequestMessage();
        var info = Config.ApiMethods.FirstOrDefault(m => m.Id == $"{typeof(T).Name}.{method.Name}");
        if (info == null)
            return request;

        var auth = await ServiceFactory.CreateAsync<IAuthStateProvider>();
        var user = await auth.GetUserAsync();
        request.Headers.Add(Constants.KeyToken, user?.Token);
        
        var queries = new List<string>();
        foreach (var param in info.Parameters)
        {
            var value = arguments[param.Position] ?? default;
            if (info.HttpMethod == HttpMethod.Get)
            {
                queries.Add($"{param.Name}={value}");
            }
            else
            {
                if (param.ParameterType.IsClass)
                {
                    var json = JsonSerializer.Serialize(value);
                    request.Content = new StringContent(json, Encoding.Default, "application/json");
                }
            }
        }

        var url = info.Route;
        if (queries.Count > 0)
            url += $"?{string.Join('&', queries)}";
        request.RequestUri = new(url, UriKind.Relative);
        request.Method = info.HttpMethod;
        //Console.WriteLine($"ReqUrl={request.RequestUri}");
        return request;
    }
}