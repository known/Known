using Known.Web.Mvc;

namespace Known.Web
{
    public abstract class BaseController : Core.BaseController
    {
        protected ActionResult GridView()
        {
            return new GridViewResult(Context);
        }
    }
}