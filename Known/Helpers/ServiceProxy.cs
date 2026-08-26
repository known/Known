namespace Known.Helpers;

/// <summary>
/// 服务动态代理类，用于自动生成HTTP客户端代理，支持一段式服务在Wasm模式下运行。
/// 通过DispatchProxy拦截接口方法调用，自动将方法名+参数转换为HTTP请求。
/// URL路由约定与服务端ApiConvention保持一致：/{ServiceName}/{MethodName}，
/// 其中ServiceName去掉"Service"后缀，MethodName去掉"Async"后缀。
/// </summary>
/// <typeparam name="T">服务接口类型。</typeparam>
internal class ServiceProxy<T> : DispatchProxy where T : class
{
    private HttpClient _http;
    private Context _context;

    internal void Initialize(HttpClient http, Context context)
    {
        _http = http;
        _context = context;
    }

    public Context Context
    {
        get => _context;
        set => _context = value;
    }

    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        var methodName = targetMethod.Name;

        if (methodName == "get_Context")
            return _context;
        if (methodName == "set_Context")
        {
            _context = (Context)args[0];
            return null;
        }

        var returnType = targetMethod.ReturnType;
        if (!returnType.IsGenericType || returnType.GetGenericTypeDefinition() != typeof(Task<>))
            return null;

        var resultType = returnType.GetGenericArguments()[0];
        var method = typeof(ServiceProxy<T>).GetMethod(nameof(InvokeAsync), BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(resultType);
        return method.Invoke(this, [targetMethod, args]);
    }

    private async Task<TResult> InvokeAsync<TResult>(MethodInfo targetMethod, object[] args)
    {
        var url = BuildUrl(targetMethod, args);
        SetAuthToken();

        try
        {
            var httpMethod = GetHttpMethod(targetMethod);
            if (httpMethod == "GET")
            {
                if (targetMethod.GetParameters().Length > 0)
                {
                    var parameters = targetMethod.GetParameters().Select((p, i) => $"{p.Name}={Uri.EscapeDataString(args[i]?.ToString() ?? "")}");
                    var queryString = string.Join("&", parameters);
                    url = $"{url}?{queryString}";
                }
                url = _http.GetRequestUrl(url);
                //Console.WriteLine($"TYPE：{typeof(TResult).Name}");
                if (typeof(TResult) == typeof(string))
                    return (TResult)(object)await _http.GetStringAsync(url);
                return await _http.GetFromJsonAsync<TResult>(url);
            }
            else
            {
                url = _http.GetRequestUrl(url);
                var body = args.Length == 1 ? args[0] : args;
                var response = await _http.PostAsJsonAsync(url, body);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResult>();
            }
        }
        catch (Exception ex)
        {
            Logger.Error(LogTarget.FrontEnd, new UserInfo { Name = "Proxy" }, $"URL：{url}\r\n{ex}");
            return default;
        }
    }

    /// <summary>
    /// 构建请求URL，与服务端ApiConvention保持一致。
    /// 例如：IHomeService.QueryHomesAsync → /Home/QueryHomes
    /// </summary>
    private static string BuildUrl(MethodInfo method, object[] args)
    {
        var serviceName = typeof(T).Name;
        if (serviceName.StartsWith('I'))
            serviceName = serviceName[1..];
        if (serviceName.EndsWith("Service"))
            serviceName = serviceName[..^"Service".Length];

        var methodName = method.Name;
        if (methodName.EndsWith("Async"))
            methodName = methodName[..^"Async".Length];

        return $"/{serviceName}/{methodName}";
    }

    /// <summary>
    /// 根据方法名和参数类型确定HTTP方法，与服务端ApiConvention一致。
    /// Get开头且参数均为简单类型 → GET，否则 → POST。
    /// </summary>
    private static string GetHttpMethod(MethodInfo method)
    {
        if (!method.Name.StartsWith("Get"))
            return "POST";

        foreach (var param in method.GetParameters())
        {
            if (param.ParameterType.IsClass && param.ParameterType != typeof(string))
                return "POST";
        }
        return "GET";
    }

    private void SetAuthToken()
    {
        var user = _context?.CurrentUser;
        var token = user != null ? user.Token : "none";
        _http.DefaultRequestHeaders.Remove(Constants.KeyToken);
        _http.DefaultRequestHeaders.Add(Constants.KeyToken, token);
    }
}
