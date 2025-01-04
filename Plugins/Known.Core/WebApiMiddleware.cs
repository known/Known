namespace Known;

class WebApiMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext httpContext, Context context)
    {
        if (!httpContext.Request.Headers.TryGetValue(Constants.KeyToken, out var token))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Unauthorized");
            return;
        }

        context.CurrentUser = Cache.GetUserByToken(token);
        if (context.CurrentUser == null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Invalid Token");
            return;
        }

        await _next(httpContext);
    }
}