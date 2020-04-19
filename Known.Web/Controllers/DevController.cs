using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class DevController : ControllerBase
    {
        public ActionResult Index()
        {
            var assembly = GetType().Assembly;
            var html = Utils.GetResource(assembly, "DevView.html");
            return Content(html);
        }
    }
}
