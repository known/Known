using System.Web.Mvc;
using System.Web.Security;

namespace Known.Web.Filters
{
    /// <summary>
    /// 用户身份认证特性。
    /// </summary>
    public class LoginAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 身份认证校验。
        /// </summary>
        /// <param name="filterContext">认证请求上下文。</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
                return;

            if (httpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new { IsScript = true, Script = "<script>showLoginForm('Login')</script>" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                var loginUrl = $"{FormsAuthentication.LoginUrl}?backUrl={httpContext.Request.RawUrl}";
                filterContext.Result = new RedirectResult(loginUrl);
            }
        }
    }
}