using System.Web.Mvc;

namespace Known.WebMvc.Controllers
{
    public class DevToolController : AuthorizeController
    {
        public ActionResult DevToolView()
        {
            return View();
        }
    }
}