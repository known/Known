namespace Known;

public abstract class HttpInterceptor<T>(IServiceProvider provider) where T : class
{
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

    protected IServiceProvider ServiceProvider { get; } = provider;
    protected abstract HttpClient CreateClient();

    protected async Task<object> SendAsync(MethodInfo method)
    {
        using var client = CreateClient();
        var request = CreateRequestMessage(method);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        if (stream == null || stream.Length == 0)
            return default;

        return await JsonSerializer.DeserializeAsync(stream, method.ReturnType, jsonOptions);
    }

    protected async Task<TResult> SendAsync<TResult>(MethodInfo method)
    {
        using var client = CreateClient();
        var request = CreateRequestMessage(method);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        if (stream == null || stream.Length == 0)
            return default;

        return await JsonSerializer.DeserializeAsync<TResult>(stream, jsonOptions);
    }

    private HttpRequestMessage CreateRequestMessage(MethodInfo method)
    {
        var request = new HttpRequestMessage
        {
            Method = method.Name.StartsWith("Get") ? HttpMethod.Get : HttpMethod.Post
        };

        //var sb = new StringBuilder();
        //var apiRoute = Converter.GetApiRoute(typeof(T), method);

        //if (!apiRoute.StartsWith("/"))
        //    sb.Append('/');
        //sb.Append(apiRoute);

        //var queryParameters = new List<string>();
        //var parameters = Converter.GetParameters();
        //var key = DefaultHttpRemotingResolver.GetMethodCacheKey(method);
        //var parameterInfoList = parameters[key];

        //foreach (var param in parameterInfoList)
        //{
        //    var name = param.GetParameterNameInHttpRequest();
        //    var value = invocation.Arguments[param.Position] ?? default;

        //    switch (param.Type)
        //    {
        //        case HttpParameterType.FromBody:
        //            var json = JsonSerializer.Serialize(value);
        //            request.Content = new StringContent(json, Encoding.Default, "application/json");
        //            break;
        //        case HttpParameterType.FromHeader:
        //            request.Headers.Add(name, value?.ToString());
        //            break;
        //        case HttpParameterType.FromForm:
        //            var arguments = new Dictionary<string, string>();
        //            if (param.ValueType != typeof(string) && param.ValueType.IsClass)
        //            {
        //                foreach (var property in param.ValueType.GetProperties())
        //                {
        //                    if (property.CanRead)
        //                    {
        //                        name = property.Name;
        //                        value = property.GetValue(value)?.ToString();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                arguments.Add(name!, value?.ToString());
        //            }
        //            request.Content = new FormUrlEncodedContent(arguments);
        //            break;
        //        case HttpParameterType.FromRoute://路由替换
        //            var match = Regex.Match(sb.ToString(), @"{\w+}");
        //            if (match.Success)
        //                sb.Replace(match.Value, match.Result(value?.ToString()));
        //            break;
        //        case HttpParameterType.FromQuery:
        //            if (param.ValueType != typeof(string) && param.ValueType.IsClass)
        //            {
        //                foreach (var property in param.ValueType.GetProperties())
        //                {
        //                    if (!property.CanRead)
        //                        continue;

        //                    if (property.TryGetCustomAttribute<JsonIgnoreAttribute>(out _))
        //                        continue;

        //                    if (property.TryGetCustomAttribute<JsonPropertyNameAttribute>(out var jsonNameProperty))
        //                        name = jsonNameProperty!.Name;
        //                    else
        //                        name = property.Name;

        //                    var propertyValue = property.GetValue(value);
        //                    if (propertyValue is not null)
        //                    {
        //                        queryParameters.Add($"{name}={propertyValue}");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                queryParameters.Add($"{name}={value}");
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //var uriString = $"{sb}{(queryParameters.Count > 0 ? $"?{string.Join("&", queryParameters)}" : String.Empty)}";
        //request.RequestUri = new(uriString, UriKind.Relative);
        return request;
    }
}