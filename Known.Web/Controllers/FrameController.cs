using System.Web.Mvc;
using Known.Platform;
using Known.Web.Models;

namespace Known.Web.Controllers
{
    public class FrameController : AuthorizeController
    {
        public ActionResult Index(string mid)
        {
            var module = Api.Get<Module>("/api/module/getmodule", new { mid });
            if (module == null)
                return Content("模块不存在！");

            switch (module.ViewType)
            {
                case Platform.ViewType.DataGridView:
                    return View("DataGridView", new DataGridViewModel(module));
                case Platform.ViewType.TreeGridView:
                    return View("TreeGridView", new TreeGridViewModel(module));
                case Platform.ViewType.TabPageView:
                    return View("TabPageView", new TabPageViewModel(module));
                case Platform.ViewType.SplitPageView:
                    return View("SplitPageView", new SplitPageViewModel(module));
                default:
                    return View(module);
            }
        }
    }
}