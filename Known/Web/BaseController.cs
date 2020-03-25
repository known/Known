using System.Web.Mvc;
using Known.Core;

namespace Known.Web
{
    public class BaseController : Controller
    {
        public PlatformService Platform { get; } = new PlatformService();

        public string UserName
        {
            get { return User.Identity.Name; }
        }

        protected ActionResult ErrorResult(string message, object data = null)
        {
            return JsonResult(new { ok = false, message, data });
        }

        protected ActionResult SuccessResult(string message, object data = null)
        {
            return JsonResult(new { ok = true, message, data });
        }

        protected ActionResult JsonResult(object value)
        {
            var json = Serializer.ToJson(value);
            return Content(json, "application/json");
        }
    }
}
