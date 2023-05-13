using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Known.Core.Filters;

class AuthActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var action = context.ActionDescriptor as ControllerActionDescriptor;
        var attribute = action.MethodInfo.GetCustomAttribute(typeof(AllowAnonymousAttribute), false);
        if (attribute != null)
            return;

        var request = context.HttpContext.Request;
        var token = request.Headers[Constants.KeyToken].FirstOrDefault();
        var user = UserService.GetUserByToken(token);
        if (user == null)
        {
            context.Result = new JsonResult(new { Status = HttpStatusCode.Unauthorized, Message = "Not authorized!" });
            return;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}