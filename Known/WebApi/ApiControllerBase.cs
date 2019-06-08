using System.Web.Http;
using Known.Core;
using Known.Extensions;
using Known.Web;

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

        #region View
        /// <summary>
        /// 查询分页数据对象。
        /// </summary>
        /// <param name="data">查询条件对象。</param>
        /// <returns>分页数据对象。</returns>
        [HttpPost]
        public object QueryDatas(CriteriaData data)
        {
            var criteria = data.ToPagingCriteria();
            var result = QueryDatas(criteria);
            return ApiResult.ToPageData(result);
        }

        /// <summary>
        /// 查询分页数据对象。
        /// </summary>
        /// <param name="criteria">查询条件对象。</param>
        /// <returns>分页查询结果。</returns>
        protected virtual PagingResult QueryDatas(PagingCriteria criteria)
        {
            return null;
        }

        /// <summary>
        /// 删除一个或多个实体对象。
        /// </summary>
        /// <param name="data">实体对象 Id 数组。</param>
        /// <returns>删除结果。</returns>
        [HttpPost]
        public object DeleteDatas([FromBody]string data)
        {
            var ids = data.FromJson<string[]>();
            var result = DeleteDatas(ids);
            return ApiResult.Result(result);
        }

        /// <summary>
        /// 删除一个或多个实体对象。
        /// </summary>
        /// <param name="ids">实体对象 Id 数组。</param>
        /// <returns>删除结果。</returns>
        protected virtual Result DeleteDatas(string[] ids)
        {
            return null;
        }

        /// <summary>
        /// 导入实体对象。
        /// </summary>
        /// <returns>导入结果。</returns>
        [HttpPost]
        public object ImportDatas()
        {
            return null;
        }
        #endregion

        #region Form
        /// <summary>
        /// 获取实体对象。
        /// </summary>
        /// <param name="id">实体 id。</param>
        /// <returns>实体对象。</returns>
        public virtual object GetData(string id)
        {
            return null;
        }

        /// <summary>
        /// 保存实体对象。
        /// </summary>
        /// <param name="data">实体对象 JSON。</param>
        /// <returns>保存结果。</returns>
        [HttpPost]
        public object SaveData([FromBody]string data)
        {
            var model = data.FromJson<dynamic>();
            var result = SaveData(model);
            return ApiResult.Result(result);
        }

        /// <summary>
        /// 保存实体对象。
        /// </summary>
        /// <param name="model">实体对象。</param>
        /// <returns>保存结果。</returns>
        protected virtual Result SaveData(dynamic model)
        {
            return null;
        }
        #endregion
    }
}