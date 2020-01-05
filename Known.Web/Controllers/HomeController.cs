using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class HomeController : MvcControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Welcome()
        {
            return View();
        }

        [AllowAnonymous, Route("login")]
        public ActionResult Login()
        {
            PlatformService.SignOut(UserName);
            return View();
        }
    }
}