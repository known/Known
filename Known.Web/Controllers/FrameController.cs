using System.Web.Mvc;
using Known.Platform;
using Known.Web.ViewModels;

namespace Known.Web.Controllers
{
    public class FrameController : AuthorizeController
    {
        public ActionResult Index(string mid)
        {
            var result = Api.Get<ApiResult>("/api/user/getmodule", new { mid });
            if (result.Status == 1)
                return ErrorResult(result.Message);

            var module = result.Data as Module;
            switch (module.ViewType)
            {
                case ModuleViewType.DataGridView:
                    return DataGridView(module);
                case ModuleViewType.TreeGridView:
                    return TreeGridView(module);
                case ModuleViewType.TabPageView:
                    return TabPageView(module);
                case ModuleViewType.SplitPageView:
                    return SplitPageView(module);
                default:
                    return View(module);
            }
        }

        public ActionResult DataGridView(Module module)
        {
            return View(new DataGridViewModel(module));
        }

        public ActionResult TreeGridView(Module module)
        {
            return View(new TreeGridViewModel(module));
        }

        public ActionResult TabPageView(Module module)
        {
            return View(new TabPageViewModel(module));
        }

        public ActionResult SplitPageView(Module module)
        {
            return View(new SplitPageViewModel(module));
        }
    }
}