using System.Linq;
using System.Web.Mvc;
using Known.Core;
using Known.Core.Services;

namespace Known.Web.Controllers
{
    public class OrganizationController : ControllerBase
    {
        private OrganizationService Service { get; } = new OrganizationService();

        #region View
        public ActionResult GetOrganizationTree()
        {
            var datas = Service.GetOrganizations();
            return JsonResult(datas.Select(d => new
            {
                id = d.Id,
                pid = d.ParentId,
                title = d.Name,
                spread = string.IsNullOrWhiteSpace(d.ParentId),
                data = d
            }));
        }

        [HttpPost]
        public ActionResult QueryOrganizations(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryOrganizations(c));
        }

        [HttpPost]
        public ActionResult DeleteOrganizations(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteOrganizations(d));
        }
        #endregion

        #region Form
        public ActionResult GetOrganization(string id)
        {
            return JsonResult(Service.GetOrganization(id));
        }

        [HttpPost]
        public ActionResult SaveOrganization(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveOrganization(d));
        }
        #endregion
    }
}