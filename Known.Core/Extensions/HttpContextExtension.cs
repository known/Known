namespace Known.Core.Extensions;

public static class HttpContextExtension
{
    public static string GetHostUrl(this HttpContext context)
    {
        return context.Request.Scheme + "://" + context.Request.Host;
    }

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