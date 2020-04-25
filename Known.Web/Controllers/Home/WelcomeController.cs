using System.Web.Mvc;
using Known.Core.Services;

namespace Known.Web.Controllers
{
    public class WelcomeController : ControllerBase
    {
        private WelcomeService Service { get; } = new WelcomeService();

        public ActionResult GetTodoLists()
        {
            var data = new CriteriaData();
            return QueryPagingData(data, c => Service.GetTodoLists(c));
        }

        public ActionResult GetCompanyNews()
        {
            var data = new CriteriaData();
            return QueryPagingData(data, c => Service.GetCompanyNews(c));
        }

        public ActionResult GetShortCuts()
        {
            return null;
        }
    }
}