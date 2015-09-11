using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Known.SLite;

namespace Known.Web.Admin
{
    public partial class Setting : AdminPage1
    {
        protected SiteSetting setting;

        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle("网站设置");
            setting = SiteSetting.Instance;

            if (Request.IsPost())
            {
                setting.SiteName = Request.Get<string>("SiteName");
                setting.SiteDescription = Request.Get<string>("SiteDescription");
                setting.MetaKeywords = Request.Get<string>("MetaKeywords");
                setting.MetaDescription = Request.Get<string>("MetaDescription");
                setting.Enabled = Request.Get<string>("Enabled") == "on";
                setting.FooterHtml = Request.Get<string>("FooterHtml");
                setting.Save();
                ShowMessage("保存成功！");
            }
        }
    }
}