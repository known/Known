using System.Web.Mvc;
using Known.Core.Services;

namespace Known.WebMvc.Controllers.Develop
{
    public class DevDemoController : AuthorizeController
    {
        private DevDemoService Service
        {
            get { return Container.Resolve<DevDemoService>(); }
        }

        #region DemoGrid
        public ActionResult QueryUsers()
        {
            var criteria = GetPagingCriteria();
            var result = Service.QueryUsers(criteria);
            return PageResult(result);
        }
        #endregion
    }
}