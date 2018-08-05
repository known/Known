using System.Web.Http;
using Known.Extensions;
using Known.Web;

namespace Known.WebApi.Controllers
{
    public class DevToolController : BaseApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public ApiResult QueryDatas(PagingCriteria criteria)
        {
            var sql = criteria.Parameters.querySql.ToString();
            criteria.Parameters = null;
            var result = Context.Database.QueryPageTable(sql, criteria) as PagingResult;
            //var data = new DataTable();
            //data.Columns.Add("Id");
            //data.Columns.Add("Test");
            //data.Columns.Add("Name");
            //for (int i = 0; i < 20; i++)
            //{
            //    data.Rows.Add(Utils.NewGuid, $"Test{i}", $"Name{i}");
            //}

            return ApiResult.Success(new
            {
                total = result.TotalCount,
                data = result.PageData
            });
        }
    }
}