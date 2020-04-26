using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class SystemController : ControllerBase
    {
        #region View
        public ActionResult ModuleView()
        {
            return ViewResult();
        }

        public ActionResult RoleView()
        {
            return ViewResult();
        }

        public ActionResult UserView()
        {
            return ViewResult();
        }

        public ActionResult DictionaryView()
        {
            return ViewResult();
        }
        #endregion
    }
}
