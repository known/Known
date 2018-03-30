using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
    }
}