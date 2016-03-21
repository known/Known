using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Known.SLite;

namespace Known.Web.Admin
{
    public partial class Default1 : AdminPage1
    {
        protected IEnumerable<Member> Members = new List<Member>();

        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle("管理中心");
            Members = Entity.FindAll<Member>().OrderByDescending(l => l.InsertTime);
        }
    }
}