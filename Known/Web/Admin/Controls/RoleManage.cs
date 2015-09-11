using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Known.Models;

namespace Known.Web.Admin.Controls
{
    public class RoleManage : UserControlBase
    {
        protected List<RoleInfo> Roles = null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Roles = AppContext.RoleService.GetRoles();
        }
    }
}
