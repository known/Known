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
    public static Task<string> GetAsync(this HttpClient http, string url)
    {
        return http.GetStringAsync(url);
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
        return http.GetFromJsonAsync<TResult>(url);
    }

    /// <summary>
    /// 异步调用服务端无参数的URL。
    /// </summary>
    /// <param name="http">HTTP客户端对象。</param>
    /// <param name="url">远程URL。</param>
    /// <returns>调用结果。</returns>
    public static async Task<Result> PostAsync(this HttpClient http, string url)
    {
        var response = await http.PostAsync(url, null);
        return await response.Content.ReadFromJsonAsync<Result>();
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
        var response = await http.PostAsJsonAsync(url, data);
        return await response.Content.ReadFromJsonAsync<TResult>();
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
        var response = await http.PostAsync(url, data);
        return await response.Content.ReadFromJsonAsync<Result>();
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
        return http.PostAsync<T, Result>(url, data);
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
        return http.PostAsync<PagingCriteria, PagingResult<T>>(url, criteria);
    }
}