using System.Web.Mvc;

namespace Known.WebMvc.Controllers
{
    public class DevelopController : AuthorizeController
    {
        public ActionResult DevelopView()
        {
            return View();
        }
    }
}