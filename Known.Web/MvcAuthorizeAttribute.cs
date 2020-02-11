using System;
using System.Web.Mvc;
using System.Web.Security;
using Known.Web.Extensions;

namespace Known.Web
{
    /// <summary>
    /// Mvc 登录授权特性类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class MvcAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 操作方法被授权时的触发动作。
        /// </summary>
        /// <param name="filterContext">执行操作上下文对象。</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.IsUseAttributeOf<AllowAnonymousAttribute>())
                return;

            var httpContext = filterContext.RequestContext.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
                return;

            if (httpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult()
                {
                    Data = new { timeout = true, message = "用户未登录！" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                filterContext.Result = new EmptyResult();
                var loginUrl = FormsAuthentication.LoginUrl;
                var script = "<script>top.location='" + loginUrl + "';</script>";
                httpContext.Response.Write(script);
                httpContext.Response.End();
            }
        }
    }
}