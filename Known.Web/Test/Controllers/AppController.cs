using Known.Web.Mvc;
using Known.Web.Services;

namespace Known.Web.Controllers
{
    /// <summary>
    /// 应用程序控制器类。
    /// </summary>
    public class AppController : BaseController
    {
        private AppService Service
        {
            get { return LoadService<AppService>(); }
        }

        /// <summary>
        /// 获取开发平台应用程序对象列表。
        /// </summary>
        /// <returns>应用程序对象列表。</returns>
        public ActionResult GetAppList()
        {
            return Json(Service.GetAppList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteApp(string id)
        {
            var result = Service.DeleteApp(id);
            return ValidateResult(result);
        }
    }
}
