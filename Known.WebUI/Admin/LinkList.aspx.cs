using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Known.SLite;
using Known.Web.Extensions;

namespace Known.Web.Admin
{
    public partial class LinkList : AdminPage<Link>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle("链接管理");

            HandleGetRequest();

            if (Request.IsPost())
            {
                Model.Name = Request.Get<string>("Name");
                Model.Url = Request.Get<string>("Url");
                Model.Description = Request.Get<string>("Description");
                Model.Position = Request.Get<int>("Position");
                Model.DisplayOrder = Request.Get<int>("DisplayOrder");
                Model.IsShow = Request.Get<string>("IsShow") == "on";
                Model.IsNewWindow = Request.Get<string>("IsNewWindow") == "on";
                HandlePostRequest();
            }

            Models = Entity.FindAll<Link>().OrderBy(l => l.DisplayOrder);
        }
    }
}