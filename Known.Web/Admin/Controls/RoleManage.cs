using Known.Services;
using System;
using System.Collections.Generic;

namespace Known.Web.Admin.Controls
{
    public class RoleManage : UserControlBase
    {
        protected List<RoleInfo> Roles = null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Roles = AppContext.LoadService<IRoleService>().GetRoles();
        }
    }
}
