namespace Known.Core.Extensions;

/// <summary>
/// Http上下文扩展类。
/// </summary>
public static class HttpContextExtension
{
    /// <summary>
    /// 获取服务器主机URL地址，格式：http(s)://localhost。
    /// </summary>
    /// <param name="context">Http上下文对象。</param>
    /// <returns></returns>
    public static string GetHostUrl(this HttpContext context)
    {
        return context.Request.Scheme + "://" + context.Request.Host;
    }

    /// <summary>
    /// 判断当前请求是否是移动浏览器请求。
    /// </summary>
    /// <param name="context">Http上下文对象。</param>
    /// <returns></returns>
    /// <exception cref="Exception">请求对象若不存在，确认服务器是否开启 WebSocket。</exception>
    public static bool CheckMobile(this HttpContext context)
    {
        if (context == null || context.Request == null)
            throw new Exception("Server WebSocket not enabled!");

        var agent = context.Request.Headers["User-Agent"].ToString();
        if (string.IsNullOrWhiteSpace(agent))
            agent = context.Request.Headers["X-Forwarded-For"].ToString();
        return Utils.CheckMobile(agent);
    }
}