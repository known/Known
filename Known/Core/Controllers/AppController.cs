using System.Web.Http;
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
