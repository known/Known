using Known.Web.Mvc;

namespace Known.Web.Controllers
{
    [Module("系统管理", "fa fa-cogs")]
    public class SystemController : ControllerBase
    {
        [Module(1, "角色管理", "fa fa-users")]
        public ActionResult RoleView()
        {
            return View();
        }

        [Module(2, "用户管理", "fa fa-user")]
        public ActionResult UserView()
        {
            return View();
        }
    }
}