using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Known.Filters;

class AuthActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var action = context.ActionDescriptor as ControllerActionDescriptor;
        var attribute = action.MethodInfo.GetCustomAttribute(typeof(AllowAnonymousAttribute), false);
        if (attribute != null)
            return;

        var request = context.HttpContext.Request;
        if (!request.Headers.TryGetValue(Constants.KeyToken, out var token))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var user = Cache.GetUserByToken(token);
        if (user == null)
        {
            //context.Result = new JsonResult(new { Status = HttpStatusCode.Unauthorized, Message = "Not authorized!" });
            context.Result = new UnauthorizedResult();
            return;
        }

        if (context.Controller is IService)
            (context.Controller as IService).Context.CurrentUser = user;
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}