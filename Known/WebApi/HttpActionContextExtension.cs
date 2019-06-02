using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using Known.Web;

namespace Known.WebApi
{
    /// <summary>
    /// 操作上下文扩展类。
    /// </summary>
    static class HttpActionContextExtension
    {
        /// <summary>
        /// 判断操作方法是否使用指定类型的特性。
        /// </summary>
        /// <typeparam name="T">特性类型。</typeparam>
        /// <param name="actionContext">执行操作上下文对象。</param>
        /// <returns>是否返回 True，否则返回 False。</returns>
        public static bool IsUseAttributeOf<T>(this HttpActionContext actionContext) where T : Attribute
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<T>(true).Count > 0 ||
                   actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<T>(true).Count > 0;
        }

        /// <summary>
        /// 创建执行操作上下文的响应信息。
        /// </summary>
        /// <param name="actionContext">执行操作上下文对象。</param>
        /// <param name="statusCode">响应状态码。</param>
        public static void CreateResponse(this HttpActionContext actionContext, HttpStatusCode statusCode)
        {
            actionContext.Response = actionContext.Request.CreateResponse(statusCode);
        }

        /// <summary>
        /// 创建执行操作上下文的响应信息。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="actionContext">执行操作上下文对象。</param>
        /// <param name="statusCode">响应状态码。</param>
        /// <param name="value">响应的数据对象。</param>
        public static void CreateResponse<T>(this HttpActionContext actionContext, HttpStatusCode statusCode, T value)
        {
            actionContext.Response = actionContext.Request.CreateResponse(statusCode, value);
        }

        /// <summary>
        /// 创建执行操作上下文的错误响应信息。
        /// </summary>
        /// <param name="actionContext">执行操作上下文对象。</param>
        /// <param name="message">错误消息。</param>
        /// <param name="data">数据对象。</param>
        public static void CreateErrorResponse(this HttpActionContext actionContext, string message, object data = null)
        {
            var result = ApiResult.Error(message, data);
            actionContext.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}