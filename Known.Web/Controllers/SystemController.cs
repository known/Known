using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class SystemController : ControllerBase
    {
        private SystemService Service => new SystemService();

        #region View
        public ActionResult DictionaryView()
        {
            return ViewResult();
        }

        public ActionResult OrganizationView()
        {
            return ViewResult();
        }

        public ActionResult RoleView()
        {
            return ViewResult();
        }

        public ActionResult UserView()
        {
            return ViewResult();
        }

        public ActionResult LogView()
        {
            return ViewResult();
        }
        #endregion

        #region Dictionary
        public ActionResult GetCategories()
        {
            var datas = Service.GetCategories();
            return JsonResult(datas.Select(d => new
            {
                id = d.Code,
                pid = "",
                title = $"{d.Sort}.{d.Name}({d.Code})",
                data = d
            }));
        }

        [HttpPost]
        public ActionResult QueryDictionarys(CriteriaData data)
        {
            if (string.IsNullOrWhiteSpace(data.query))
                data.query = Utils.ToJson(new { Category = "" });

            return QueryPagingData(data, c => Service.QueryDictionarys(c));
        }

        [HttpPost]
        public ActionResult DeleteDictionarys(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteDictionarys(d));
        }

        public ActionResult GetDictionary(string id)
        {
            return JsonResult(Service.GetDictionary(id));
        }

        [HttpPost]
        public ActionResult SaveDictionary(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveDictionary(d));
        }
        #endregion

        #region Log
        [HttpPost]
        public ActionResult QueryLogs(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryLogs(c));
        }

        [HttpPost]
        public ActionResult DeleteLogs(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteLogs(d));
        }
        #endregion

        #region Organization
        public ActionResult GetOrganizations()
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

        #region Role
        [HttpPost]
        public ActionResult QueryRoles(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryRoles(c));
        }

        [HttpPost]
        public ActionResult DeleteRoles(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteRoles(d));
        }

        public ActionResult GetRole(string id)
        {
            return JsonResult(Service.GetRole(id));
        }

        [HttpPost]
        public ActionResult SaveRole(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveRole(d));
        }

        public ActionResult GetRoleModules(string roleId)
        {
            var modules = Service.GetRoleModules(roleId);
            return JsonResult(modules.Select(m => m.ToTree()));
        }

        public ActionResult SaveRoleModules(string roleId, string data)
        {
            return PostAction<List<string>>(data, d => Service.SaveRoleModules(roleId, d));
        }
        #endregion

        #region User
        [HttpPost]
        public ActionResult QueryUsers(CriteriaData data)
        {
            if (string.IsNullOrWhiteSpace(data.query))
                data.query = Utils.ToJson(new { OrgNo = "" });

            return QueryPagingData(data, c => Service.QueryUsers(c));
        }

        [HttpPost]
        public ActionResult DeleteUsers(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteUsers(d));
        }

        [HttpPost]
        public ActionResult SetUserPwds(string data)
        {
            return PostAction<string[]>(data, d => Service.SetUserPwds(d));
        }

        [HttpPost]
        public ActionResult EnableUsers(string data, int enable)
        {
            return PostAction<string[]>(data, d => Service.EnableUsers(d, enable));
        }

        public ActionResult GetUser(string id)
        {
            return JsonResult(Service.GetUser(id));
        }

        [HttpPost]
        public ActionResult SaveUser(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveUser(d));
        }

        class RoleValue
        {
            public string value { get; set; }
            public string title { get; set; }

            internal static RoleValue Create(string value, string title)
            {
                return new RoleValue { value = value, title = title };
            }
        }

        public ActionResult GetUserRoles(string userId)
        {
            var roles = Service.GetRoles().Select(r => RoleValue.Create(r.Id, r.Name));
            var value = Service.GetUserRoles(userId);
            return JsonResult(new { roles, value });
        }

        [HttpPost]
        public ActionResult SaveUserRoles(string userId, string data)
        {
            return PostAction<List<RoleValue>>(data, d =>
            {
                var roleIds = d.Select(r => r.value).ToList();
                return Service.SaveUserRoles(userId, roleIds);
            });
        }
        #endregion
    }
}
