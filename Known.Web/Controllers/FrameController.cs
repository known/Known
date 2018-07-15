using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class FrameController : AuthorizeController
    {
        public ActionResult DemoView()
        {
            return View();
        }
    }
}