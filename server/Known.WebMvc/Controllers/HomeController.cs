using System.Web.Mvc;
using Known.WebMvc.Filters;

namespace Known.WebMvc.Controllers
{
    public class HomeController : BaseController
    {
        [LoginAuthorize]
        public ActionResult Index()
        {
            ViewBag.UserName = CurrentUser.Name;
            return View();
        }

        [Route("login")]
        public ActionResult Login(string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            return View();
        }

        [Route("register")]
        public ActionResult Register()
        {
            return View();
        }
    }
}