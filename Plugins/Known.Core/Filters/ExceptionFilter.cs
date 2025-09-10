using Microsoft.AspNetCore.Mvc.Filters;

namespace Known.Filters;

class ExceptionFilter : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        if (!context.ExceptionHandled)
        {
            var request = context.HttpContext.Request;
            request.Headers.TryGetValue(Constants.KeyToken, out var token);
            var user = Cache.GetUserByToken(token);
            Logger.Exception(LogTarget.BackEnd, user, context.Exception);
            var result = Result.Error(context.Exception.Message);
            context.Result = new JsonResult(result);
        }
        context.ExceptionHandled = true;
        return Task.CompletedTask;
    }
}