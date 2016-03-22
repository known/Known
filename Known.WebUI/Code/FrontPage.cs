using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Known.Web
{
    public class FrontPage : PageBase
    {
        public string FormatUrl(string url)
        {
            return url.Replace("${siteurl}", SiteUrl); //KConfig.SiteUrl);
        }

        public string FormatEnum(Enum type)
        {
            var value = Convert.ToInt32(type);
            if (value == 0)
                return "管理员";
            return "普通用户";
        }

        public string GetErrorsHtml(List<string> errors)
        {
            return string.Format("<div class=\"errors container red\">{0}</div>", string.Join("<br/>", errors.ToArray()));
        }
    }
}