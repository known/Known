using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Known.Filters;

class LogActionFilter : IActionFilter
{
    private StringValues token;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        request.Headers.TryGetValue(Constants.KeyToken, out token);
    }

    public async void OnActionExecuted(ActionExecutedContext context)
    {
        var ctx = context.HttpContext.RequestServices.GetRequiredService<Context>();
        ctx.CurrentUser = Cache.GetUserByToken(token);
        await VisitLog.AddLogAsync(context.HttpContext, ctx);
    }
}