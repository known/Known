using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Known.Models;

namespace Known
{
    public class Utils
    {
        public static string GetUrl(string url)
        {
            return KConfig.SitePath + url;
        }

        public static string GetAdminUrl(string url)
        {
            return KConfig.AdminPath + url;
        }

        public static string GetAdminMenuUrl(MenuInfo menu)
        {
            if (menu == null)
                return string.Empty;

            var url = menu.Url;
            if (string.IsNullOrEmpty(url))
                url = "GenericPage.aspx";
            url = url.TrimEnd('/');
            url = GetAdminUrl(url);
            return url;
        }

        public static string HtmlEncode(string value)
        {
            return value.HtmlEncode();
        }
    }
}
