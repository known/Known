using System.Web.Helpers;
using System.Web.Mvc;

namespace Known.WebMvc.Filters
{
    public class AntiForgeryAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            if (request.HttpMethod != "POST")
                return;

            if (request.IsAjaxRequest())
            {
                var antiForgeryCookie = request.Cookies[AntiForgeryConfig.CookieName];
                var cookieValue = antiForgeryCookie?.Value;
                AntiForgery.Validate(cookieValue, request.Headers["X-XSRF-TOKEN"]);
            }
            else
            {
                new ValidateAntiForgeryTokenAttribute().OnAuthorization(filterContext);
            }
        }
    }
}