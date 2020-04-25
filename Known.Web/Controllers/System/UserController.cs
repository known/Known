using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Known.Core.Services;

namespace Known.Web.Controllers
{
    public class UserController : ControllerBase
    {
        private UserService Service { get; } = new UserService();

        #region View
        [HttpPost]
        public ActionResult QueryUsers(CriteriaData data)
        {
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
        #endregion

        #region Form
        public ActionResult GetUser(string id)
        {
            return JsonResult(Service.GetUser(id));
        }

        [HttpPost]
        public ActionResult SaveUser(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveUser(d));
        }
        #endregion

        #region UserRole
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