using System.Linq;
using Known.Web.Mvc;

namespace Known.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Content("", "");
        }

        public ActionResult Welcome()
        {
            return Content("", "");
        }

        public ActionResult Page(string id)
        {
            var page = Setting.Instance.App.Pages.FirstOrDefault(p => p.Id == id);
            if (page == null)
                return Content($"页面{id}不存在！", "");

            return Content("", "");
        }

        //[AllowAnonymous, Route("login")]
        public ActionResult Login()
        {
            return Content("", "");
        }
    }
}