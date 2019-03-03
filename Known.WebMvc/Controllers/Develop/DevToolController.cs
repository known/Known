using System.Web.Mvc;
using Known.Core.Services;

namespace Known.WebMvc.Controllers.Develop
{
    public class DevToolController : AuthorizeController
    {
        private DevToolService Service
        {
            get { return Container.Resolve<DevToolService>(); }
        }

        #region DevDatabase
        public ActionResult QueryDatas(PagingCriteria criteria)
        {
            var result = Service.QueryDatas(criteria);
            return PageResult(result);
        }
        #endregion
    }
}