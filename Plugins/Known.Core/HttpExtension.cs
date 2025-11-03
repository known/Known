namespace Known;

/// <summary>
/// Http上下文扩展类。
/// </summary>
public static class HttpExtension
{
    /// <summary>
    /// 设置系统上下文的Http相关对象。
    /// </summary>
    /// <param name="http">Http上下文对象。</param>
    /// <param name="context">系统上下文对象。</param>
    public static void SetContext(this HttpContext http, UIContext context)
    {
        context.IsMobile = http.CheckMobile();
        context.IPAddress = http.Connection?.RemoteIpAddress?.ToString();
        context.Request = new WebHttpRequest(http);
        context.Response = new WebHttpResponse(http);
    }

    /// <summary>
    /// 获取服务器主机URL地址，格式：http(s)://localhost。
    /// </summary>
    /// <param name="http">Http上下文对象。</param>
    /// <returns></returns>
    public static string GetHostUrl(this HttpContext http)
    {
        return http.Request.Scheme + "://" + http.Request.Host;
    }

    /// <summary>
    /// 判断当前请求是否是移动浏览器请求。
    /// </summary>
    /// <param name="http">Http上下文对象。</param>
    /// <returns></returns>
    /// <exception cref="Exception">请求对象若不存在，确认服务器是否开启 WebSocket。</exception>
    public static bool CheckMobile(this HttpContext http)
    {
        if (http == null || http.Request == null)
            throw new Exception("Server WebSocket not enabled!");

        var agent = http.Request.Headers["User-Agent"].ToString();
        if (string.IsNullOrWhiteSpace(agent))
            agent = http.Request.Headers["X-Forwarded-For"].ToString();

        return Utils.CheckMobile(agent);
    }
}