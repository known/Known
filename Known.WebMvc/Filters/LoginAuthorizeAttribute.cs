using System.Web.Mvc;
using System.Web.Security;

namespace Known.WebMvc.Filters
{
    public class LoginAuthorizeAttribute : AuthorizeAttribute
    {
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