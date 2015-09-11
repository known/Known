using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Known.Models;

namespace Known.Web.Admin.Controls
{
    public class UserManage : UserControlBase
    {
        protected List<UserInfo> Users = null;
        protected List<RoleInfo> Roles = null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Users = AppContext.UserService.GetUsers();
            Roles = AppContext.RoleService.GetRoles();
        }

        protected string FormatRoles(string roles)
        {
            roles = "," + roles + ",";
            var roleNames = new List<string>();
            foreach (var item in AppContext.RoleService.GetRoles())
            {
                var id = string.Format(",{0},", item.Id);
                if (roles.Contains(id))
                {
                    roleNames.Add(item.Name);
                }
            }
            return string.Join(",", roleNames.ToArray());
        }
    }
}
