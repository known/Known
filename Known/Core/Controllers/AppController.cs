using System.Web.Http;
using Known.Core.Services;
using Known.Web;

namespace Known.Core.Controllers
{
    /// <summary>
    /// 应用程序控制器类。
    /// </summary>
    public class AppController : ApiControllerBase
    {
        private AppService Service
        {
            get { return LoadService<AppService>(); }
        }

        /// <summary>
        /// 获取开发平台应用程序对象列表。
        /// </summary>
        /// <returns>应用程序对象列表。</returns>
        public object GetAppList()
        {
            return Service.GetAppList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public object DeleteApp(string id)
        {
            return Service.DeleteApp(id);
        }
    }
}
