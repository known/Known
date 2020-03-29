using System.Web.Mvc;

namespace Known.Core
{
    public class DevController : Web.ControllerBase
    {
        public ActionResult Index()
        {
            var assembly = GetType().Assembly;
            var html = Utils.GetResource(assembly, "DevView.html");
            return Content(html);
        }
    }
}
