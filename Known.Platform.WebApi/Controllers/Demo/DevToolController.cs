using Known.Web;

namespace Known.Platform.WebApi.Controllers.Demo
{
    public class DevToolController : WebApiController
    {
        public ApiResult QueryDatas(PagingCriteria criteria)
        {
            var sql = criteria.Parameter.querySql.ToString();
            criteria.Parameter = null;
            var result = Database.QueryPageTable(sql, criteria) as PagingResult;
            return ApiResult.ToPageData(result);
        }
    }
}