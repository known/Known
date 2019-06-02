using System.Web.Http;
using Known.Core;

namespace Known.WebApi
{
    /// <summary>
    /// Api 控制器基类。
    /// </summary>
    public class ApiControllerBase : ApiController
    {
        /// <summary>
        /// 取得应用程序上下文对象。
        /// </summary>
        protected Context Context
        {
            get { return Context.Create(UserName); }
        }

        /// <summary>
        /// 取得系统当前登录用户名。
        /// </summary>
        protected string UserName
        {
            get { return User.Identity.Name; }
        }

        /// <summary>
        /// 取得系统当前用户是否已验证。
        /// </summary>
        protected bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        /// <summary>
        /// 取得平台服务对象。
        /// </summary>
        protected PlatformService PlatformService
        {
            get { return ObjectFactory.Create<PlatformService>(); }
        }

        /// <summary>
        /// 获取指定服务类型的对象。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <returns>服务类型对象。</returns>
        protected T LoadService<T>() where T : ServiceBase
        {
            return ObjectFactory.CreateService<T>(Context);
        }
    }
}