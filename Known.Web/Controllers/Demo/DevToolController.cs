using System.Web.Mvc;

namespace Known.Web.Controllers.Demo
{
    public class DevToolController : AuthorizeController
    {
        public ActionResult QueryDatas(PagingCriteria criteria)
        {
            var sql = criteria.Parameter.querySql.ToString();
            criteria.Parameter = null;
            var result = Database.QueryPageTable(sql, criteria) as PagingResult;
            return PageResult(result);
        }
    }
}