using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Known.Models;

namespace Known.Web.Admin.Controls
{
    public class MenuManage : UserControlBase
    {
        protected List<MenuInfo> Menus = null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Menus = AppContext.MenuService.GetMenus();
        }
    }
}
