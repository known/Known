using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class ViewController : AuthorizeController
    {
        public ActionResult DemoView()
        {
            return View();
        }
    }
}