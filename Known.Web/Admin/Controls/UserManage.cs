using Known.Services;
using System;
using System.Collections.Generic;

namespace Known.Web.Admin.Controls
{
    public class UserManage : UserControlBase
    {
        protected List<UserInfo> Users = null;
        protected List<RoleInfo> Roles = null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Users = AppContext.LoadService<IUserService>().GetUsers();
            Roles = AppContext.LoadService<IRoleService>().GetRoles();
        }

        protected string FormatRoles(string roles)
        {
            roles = "," + roles + ",";
            var roleNames = new List<string>();
            foreach (var item in AppContext.LoadService<IRoleService>().GetRoles())
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
