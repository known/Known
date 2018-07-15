using System.Web.Mvc;
using Known.Web.Filters;

namespace Known.Web.Controllers
{
    public class HomeController : BaseController
    {
        [LoginAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
    }
}