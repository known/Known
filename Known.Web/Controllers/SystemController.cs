using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class SystemController : AuthorizeController
    {
        public ActionResult ModuleView()
        {
            return View();
        }

        public ActionResult RoleView()
        {
            return View();
        }

        public ActionResult UserView()
        {
            return View();
        }
    }
}