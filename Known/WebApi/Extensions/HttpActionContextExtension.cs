using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using Known.Web;

namespace Known.WebApi.Extensions
{
    public static class HttpActionContextExtension
    {
        public static bool IsUseAttributeOf<T>(this HttpActionContext actionContext) where T : Attribute
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<T>(true).Count > 0 ||
                   actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<T>(true).Count > 0;
        }

        public static void CreateResponse(this HttpActionContext actionContext, HttpStatusCode statusCode)
        {
            actionContext.Response = actionContext.Request.CreateResponse(statusCode);
        }

        public static void CreateResponse<T>(this HttpActionContext actionContext, HttpStatusCode statusCode, T value)
        {
            actionContext.Response = actionContext.Request.CreateResponse(statusCode, value);
        }

        public static void CreateErrorResponse(this HttpActionContext actionContext, string message, object data = null)
        {
            var result = ApiResult.Error(message, data);
            actionContext.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}