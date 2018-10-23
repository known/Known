using System.Web.Http;
using Known.Web;

namespace Known.WebApi.Controllers
{
    public class DevToolController : BaseApiController
    {
        [HttpPost]
        public ApiResult QueryDatas(PagingCriteria criteria)
        {
            var sql = criteria.Parameters.querySql.ToString();
            criteria.Parameters = null;
            var result = Context.Database.QueryPageTable(sql, criteria) as PagingResult;

            return ApiResult.Success(new
            {
                total = result.TotalCount,
                data = result.PageData
            });
        }
    }
}