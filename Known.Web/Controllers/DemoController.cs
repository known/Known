using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class DemoController : AuthorizeController
    {
        public ActionResult DemoView()
        {
            return View();
        }
    }
}