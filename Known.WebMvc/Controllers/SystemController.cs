using System.Web.Mvc;

namespace Known.WebMvc.Controllers
{
    public class SystemController : AuthorizeController
    {
        public ActionResult ModuleView()
        {
            return View();
        }
    }
}