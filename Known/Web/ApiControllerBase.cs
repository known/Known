using System;
using System.Web.Http;
using Known.Core;
using Known.Extensions;

namespace Known.Web
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

        /// <summary>
        /// 查询分页数据对象。
        /// </summary>
        /// <param name="data">查询条件对象。</param>
        /// <param name="func">查询方法。</param>
        /// <returns>分页数据对象。</returns>
        protected object QueryPagingData(CriteriaData data, Func<PagingCriteria, PagingResult> func)
        {
            var criteria = data.ToPagingCriteria();
            var result = func(criteria);
            return ApiResult.ToPageData(result);
        }

        /// <summary>
        /// 处理 POST 请求的操作。
        /// </summary>
        /// <typeparam name="T">POST 的数据类型。</typeparam>
        /// <param name="data">前端提交的 JSON 数据。</param>
        /// <param name="func">操作方法。</param>
        /// <returns>操作结果。</returns>
        protected object PostAction<T>(string data, Func<T, Result> func)
        {
            var obj = data.FromJson<T>();
            if (obj == null)
                return Result.Error("不能提交空数据！");

            var result = func(obj);
            return ApiResult.Result(result);
        }
    }
}