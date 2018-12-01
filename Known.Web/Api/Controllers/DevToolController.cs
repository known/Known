namespace Known.Web.Api.Controllers
{
    public class DevToolController : BaseApiController
    {
        public ApiResult QueryDatas(PagingCriteria criteria)
        {
            var sql = criteria.Parameters.querySql.ToString();
            criteria.Parameters = null;
            var result = Database.QueryPageTable(sql, criteria) as PagingResult;

            return ApiResult.ToData(new
            {
                total = result.TotalCount,
                data = result.PageData
            });
        }
    }
}