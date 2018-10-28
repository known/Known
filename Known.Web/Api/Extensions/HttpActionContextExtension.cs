using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Known.Web.Api.Extensions
{
    public static class HttpActionContextExtension
    {
        public static bool IsUseAttributeOf<T>(this HttpActionContext actionContext) where T : Attribute
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<T>(true).Count > 0
                || actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<T>(true).Count > 0;
        }

        public static void CreateErrorResponse(this HttpActionContext actionContext, string message)
        {
            var result = ApiResult.Error(message);
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}