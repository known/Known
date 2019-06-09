using Known.WebApi;

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
        /// 获取实体对象。
        /// </summary>
        /// <param name="id">实体 id。</param>
        /// <returns>实体对象。</returns>
        public override object GetData(string id)
        {
            return null;
        }
    }
}
