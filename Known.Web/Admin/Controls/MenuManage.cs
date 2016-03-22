using Known.Services;
using System;
using System.Collections.Generic;

namespace Known.Web.Admin.Controls
{
    public class MenuManage : UserControlBase
    {
        protected List<MenuInfo> Menus = null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Menus = AppContext.LoadService<IMenuService>().GetMenus();
        }
    }
}
