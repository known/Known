﻿using Castle.DynamicProxy;

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
        invocation.ReturnValue = SendAsync(invocation.Method);
    }

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        invocation.ReturnValue = SendAsync<TResult>(invocation.Method);
    }

    public async void InterceptSynchronous(IInvocation invocation)
    {
        invocation.ReturnValue = await SendAsync(invocation.Method);
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