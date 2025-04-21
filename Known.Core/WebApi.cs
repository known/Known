namespace Known;

// 已使用AddDynamicWebApi方式生成WebApi
class WebApi
{
    internal static async Task Invoke(HttpContext ctx, ApiMethodInfo info)
    {
        UserInfo user = null;
        try
        {
            var context = ctx.RequestServices.GetRequiredService<Context>();
            var token = ctx.Request.Headers[Constants.KeyToken].ToString();
            user = Cache.GetUserByToken(token);
            context.CurrentUser = user;
            if (context.CurrentUser == null && !info.MethodInfo.IsAllowAnonymous())
            {
                await ctx.Response.WriteAsJsonAsync(Result.Error("用户登录已过期！"));
                return;
            }

            await VisitLog.AddLogAsync(ctx, context);
            var service = ctx.RequestServices.GetRequiredService(info.MethodInfo.DeclaringType) as IService;
            service.Context = context;
            var parameters = await GetParametersAsync(ctx, info);
            dynamic result = info.MethodInfo.Invoke(service, [.. parameters]);
            result.Wait();
            string text = Utils.ToJson(result.Result);
            await ctx.Response.WriteAsync(text);
        }
        catch (Exception ex)
        {
            Logger.Exception(LogTarget.BackEnd, user, ex);
            await ctx.Response.WriteAsJsonAsync(Result.Error(ex.Message));
        }
    }

    private static async Task<List<object>> GetParametersAsync(HttpContext ctx, ApiMethodInfo info)
    {
        var parameters = new List<object>();
        foreach (var item in info.Parameters)
        {
            if (ctx.Request.Method == "GET")
            {
                var value = ctx.Request.Query[item.Name].ToString();
                var parameter = Utils.ConvertTo(item.ParameterType, value, null);
                parameters.Add(parameter);
            }
            else
            {
                var parameter = await ctx.Request.ReadFromJsonAsync(item.ParameterType);
                parameters.Add(parameter);
            }
        }
        return parameters;
    }
}