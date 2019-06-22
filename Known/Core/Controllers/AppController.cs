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
    }
}
