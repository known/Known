using System.Text.Json;
using Castle.DynamicProxy;

namespace Sample.Client;

public class HttpInterceptor<T>(IServiceProvider provider) : IAsyncInterceptor where T : class
{
    private readonly IServiceProvider ServiceProvider = provider;

    public IHttpClientFactory HttpClientFactory => ServiceProvider.GetRequiredService<IHttpClientFactory>();

    public void InterceptAsynchronous(IInvocation invocation)
    {
        invocation.ReturnValue = SendAsync(invocation);
    }

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        invocation.ReturnValue = SendAsync<TResult>(invocation);
    }

    public void InterceptSynchronous(IInvocation invocation)
    {
        Send(invocation);
    }

    private void Send(IInvocation invocation)
    {
        using var client = CreateClient();
        var request = CreateRequestMessage(invocation);
        var response = client.Send(request);
        response.EnsureSuccessStatusCode();
        var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
        if (stream is not null && stream.Length > 0)
        {
            invocation.ReturnValue = JsonSerializer.Deserialize(stream, invocation.Method.ReturnType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }

    private async Task<object> SendAsync(IInvocation invocation)
    {
        using var client = CreateClient();
        var request = CreateRequestMessage(invocation);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        if (stream is not null && stream.Length > 0)
        {
            return await JsonSerializer.DeserializeAsync(stream, invocation.Method.ReturnType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).AsTask();
        }
        return default;
    }

    private async Task<TResult> SendAsync<TResult>(IInvocation invocation)
    {
        using var client = CreateClient();
        var request = CreateRequestMessage(invocation);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        if (stream is not null && stream.Length > 0)
        {
            return await JsonSerializer.DeserializeAsync<TResult>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).AsTask();
        }
        return default;
    }

    private HttpClient CreateClient()
    {
        var type = typeof(T);
        return HttpClientFactory.CreateClient(type.Name);
    }

    private HttpRequestMessage CreateRequestMessage(IInvocation invocation)
    {
        var method = invocation.Method;
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