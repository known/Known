using System.Linq;
using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Welcome()
        {
            return View();
        }

        public ActionResult Page(string id)
        {
            var page = Setting.Instance.App.Pages.FirstOrDefault(p => p.Id == id);
            if (page == null)
                return Content($"页面{id}不存在！");

            ViewBag.Id = page.Name;
            return View();
        }

        [AllowAnonymous, Route("login")]
        public ActionResult Login()
        {
            return View();
        }
    }
}