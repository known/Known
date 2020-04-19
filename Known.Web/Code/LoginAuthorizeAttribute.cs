using System;
using System.Web.Mvc;
using System.Web.Security;

namespace Known.Web
{
    class LoginAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (IsUseOf<AllowAnonymousAttribute>(filterContext))
                return;

            var httpContext = filterContext.RequestContext.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
                return;

            if (httpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new { timeout = true, msg = "登录超时！" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                var loginUrl = FormsAuthentication.LoginUrl;
                filterContext.Result = new RedirectResult(loginUrl);
            }
        }

        private static bool IsUseOf<T>(AuthorizationContext context) where T : Attribute
        {
            return context.ActionDescriptor.GetCustomAttributes(typeof(T), true).Length > 0 ||
                   context.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(T), true).Length > 0;
        }
    }
}
