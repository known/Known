using System.Net.Http.Json;

namespace Known.Extensions;

/// <summary>
/// HTTP客户端请求扩展类。
/// </summary>
public static class HttpExtension
{
    /// <summary>
    /// 根据URL异步获取服务端数据。
    /// </summary>
    /// <param name="http">HTTP客户端对象。</param>
    /// <param name="url">远程URL。</param>
    /// <returns>服务端数据。</returns>
    public static Task<string> GetTextAsync(this HttpClient http, string url)
    {
        try
        {
            url = http.GetRequestUrl(url);
            return http.GetStringAsync(url);
        }
        catch (Exception ex)
        {
            HandleException(ex, url);
            return Task.FromException<string>(ex);
        }
    }

    /// <summary>
    /// 根据URL异步获取服务端数据，转换成泛型对象。
    /// </summary>
    /// <typeparam name="TResult">泛型对象类型。</typeparam>
    /// <param name="http">HTTP客户端对象。</param>
    /// <param name="url">远程URL。</param>
    /// <returns>泛型对象。</returns>
    public static Task<TResult> GetAsync<TResult>(this HttpClient http, string url)
    {
        try
        {
            url = http.GetRequestUrl(url);
            return http.GetFromJsonAsync<TResult>(url);
        }
        catch (Exception ex)
        {
            HandleException(ex, url);
            return Task.FromException<TResult>(ex);
        }
    }

    /// <summary>
    /// 异步调用服务端无参数的URL。
    /// </summary>
    /// <param name="http">HTTP客户端对象。</param>
    /// <param name="url">远程URL。</param>
    /// <returns>调用结果。</returns>
    public static async Task<Result> PostAsync(this HttpClient http, string url)
    {
        try
        {
            url = http.GetRequestUrl(url);
            var response = await http.PostAsync(url, null);
            return await response.Content.ReadFromJsonAsync<Result>();
        }
        catch (Exception ex)
        {
            HandleException(ex, url);
            return Result.Error(ex.Message);
        }
    }

    /// <summary>
    /// 异步发送泛型类型数据搭配服务端。
    /// </summary>
    /// <typeparam name="TParam">发送数据类型。</typeparam>
    /// <typeparam name="TResult">返回数据类型。</typeparam>
    /// <param name="http">HTTP客户端对象。</param>
    /// <param name="url">远程URL。</param>
    /// <param name="data">发送的数据对象。</param>
    /// <returns>返回的数据对象。</returns>
    public static async Task<TResult> PostAsync<TParam, TResult>(this HttpClient http, string url, TParam data)
    {
        try
        {
            url = http.GetRequestUrl(url);
            var response = await http.PostAsJsonAsync(url, data);
            return await response.Content.ReadFromJsonAsync<TResult>();
        }
        catch (Exception ex)
        {
            HandleException(ex, url, data);
            return default;
        }
    }

    /// <summary>
    /// 异步发送带附件信息的数据到服务端。
    /// </summary>
    /// <param name="http">HTTP客户端对象。</param>
    /// <param name="url">远程URL。</param>
    /// <param name="data">发送的数据对象。</param>
    /// <returns>发送结果。</returns>
    public static async Task<Result> PostAsync(this HttpClient http, string url, HttpContent data)
    {
        try
        {
            url = http.GetRequestUrl(url);
            var response = await http.PostAsync(url, data);
            return await response.Content.ReadFromJsonAsync<Result>();
        }
        catch (Exception ex)
        {
            HandleException(ex, url, data);
            return Result.Error(ex.Message);
        }
    }

    /// <summary>
    /// 异步发送数据到服务端。
    /// </summary>
    /// <typeparam name="T">数据对象类型。</typeparam>
    /// <param name="http">HTTP客户端对象。</param>
    /// <param name="url">远程URL。</param>
    /// <param name="data">发送的数据对象。</param>
    /// <returns>发送结果。</returns>
    public static Task<Result> PostAsync<T>(this HttpClient http, string url, T data)
    {
        try
        {
            url = http.GetRequestUrl(url);
            return http.PostAsync<T, Result>(url, data);
        }
        catch (Exception ex)
        {
            HandleException(ex, url, data);
            return Task.FromException<Result>(ex);
        }
    }

    /// <summary>
    /// 异步获取远程分页查询结果。
    /// </summary>
    /// <typeparam name="T">数据实体类型。</typeparam>
    /// <param name="http">HTTP客户端对象。</param>
    /// <param name="url">远程URL。</param>
    /// <param name="criteria">查询条件。</param>
    /// <returns>分页查询结果。</returns>
    public static Task<PagingResult<T>> QueryAsync<T>(this HttpClient http, string url, PagingCriteria criteria)
    {
        try
        {
            url = http.GetRequestUrl(url);
            return http.PostAsync<PagingCriteria, PagingResult<T>>(url, criteria);
        }
        catch (Exception ex)
        {
            HandleException(ex, url, criteria);
            return Task.FromException<PagingResult<T>>(ex);
        }
    }

    private static string GetRequestUrl(this HttpClient http, string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return http.BaseAddress?.ToString();

        if (url.StartsWith("http"))
            return url;

        return $"{http.BaseAddress}{url}";
    }

    private static void HandleException(Exception ex, string url, object data = null)
    {
        ClientOption.Instance.OnError?.Invoke(new ErrorInfo { Url = url, Data = data, Exception = ex });
    }
}