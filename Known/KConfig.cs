using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace Known
{
    public class KConfig
    {
        private static string adminPath;
        private static string sitePath;
        private static string siteUrl;

        public const string KeyPrefix = "known";
        public static string RewriteConfig = string.Format("{0}static/config/rewrite.config", SitePath);

        public static string AdminPath
        {
            get
            {
                if (string.IsNullOrEmpty(adminPath))
                {
                    adminPath = GetSetting<string>("admin_path", "");
                    if (!adminPath.EndsWith("/"))
                    {
                        adminPath += "/";
                    }
                }

                return adminPath;
            }
        }

        public static string SitePath
        {
            get
            {
                if (string.IsNullOrEmpty(sitePath))
                {
                    sitePath = GetSetting<string>("site_path", "");
                    if (!sitePath.EndsWith("/"))
                    {
                        sitePath += "/";
                    }
                }

                return sitePath;
            }
        }

        public static string SiteUrl
        {
            get
            {
                siteUrl = "http://" + HttpContext.Current.Request.Url.Host + SitePath;
                if (HttpContext.Current.Request.Url.Port != 80)
                {
                    siteUrl = "http://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port + SitePath;
                }
                return siteUrl;
            }
        }

        public static T GetSetting<T>(string key, T nullValue)
        {
            var value = ConfigurationManager.AppSettings[key].To<T>();
            if (value == null)
            {
                return nullValue;
            }
            return value;
        }
    }
}
