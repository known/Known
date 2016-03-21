using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Known.SLite;

namespace Known.Web.Admin
{
    public partial class MemberList : AdminPage<Member>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle("会员管理");

            Models = Entity.FindAll<Member>().OrderByDescending(l => l.InsertTime);
        }
    }
}