using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Known.Web.Api.Extensions
{
    /// <summary>
    /// Http执行上下文扩展类。
    /// </summary>
    public static class HttpActionContextExtension
    {
        /// <summary>
        /// 判断是否使用指定类型的特性。
        /// </summary>
        /// <typeparam name="T">特性类型。</typeparam>
        /// <param name="actionContext">执行上下文。</param>
        /// <returns>使用则返回true，否则返回false。</returns>
        public static bool IsUseAttributeOf<T>(this HttpActionContext actionContext) where T : Attribute
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<T>(true).Count > 0
                || actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<T>(true).Count > 0;
        }

        /// <summary>
        /// 创建错误响应。
        /// </summary>
        /// <param name="actionContext">执行上下文。</param>
        /// <param name="message">错误消息。</param>
        public static void CreateErrorResponse(this HttpActionContext actionContext, string message)
        {
            var result = ApiResult.Error(message);
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}