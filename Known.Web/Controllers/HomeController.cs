using System.Web.Mvc;

namespace Known.Web.Controllers
{
    /// <summary>
    /// 主页控制器。
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 默认首页。
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}