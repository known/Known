using System.Web.Mvc;
using Known.WebMvc.Models;

namespace Known.WebMvc.Controllers
{
    public class FrameController : AuthorizeController
    {
        public ActionResult Index(string id)
        {
            var module = PlatformService.GetModule(id);
            if (module == null)
                return Content("模块不存在！");

            switch (module.ViewType)
            {
                case Known.Platform.ViewType.DataGridView:
                    return View("DataGridView", new DataGridViewModel(module));
                case Known.Platform.ViewType.TreeGridView:
                    return View("TreeGridView", new TreeGridViewModel(module));
                case Known.Platform.ViewType.TabPageView:
                    return View("TabPageView", new TabPageViewModel(module));
                case Known.Platform.ViewType.SplitPageView:
                    return View("SplitPageView", new SplitPageViewModel(module));
                default:
                    return View(module);
            }
        }
    }
}