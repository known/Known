using System.Web.Mvc;

namespace Known.Core.Controllers
{
    [Module("系统管理", "fa fa-cogs")]
    public class SystemController : Controller
    {
        [Page(1, "角色管理", "fa fa-users")]
        public ActionResult RoleView()
        {
            return View();
        }

        [Page(2, "用户管理", "fa fa-user")]
        public ActionResult UserView()
        {
            return View();
        }
    }
}
