using Known.Extensions;
using Known.Web.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Known.Web
{
    public class PageBase : Page
    {
        public virtual string VirtualPath
        {
            //get { return KConfig.SitePath; }
            get { return ""; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ValidateLogin();

            if (Request.IsGet())
            {
                //AppContext.AlertMessage = string.Empty;
                HandleGetRequest();
            }

            if (Request.IsPost())
            {
                HandlePostRequest();
            }
        }

        protected virtual void ValidateLogin()
        {
        }

        protected virtual void HandleGetRequest()
        {
        }

        protected virtual void HandlePostRequest()
        {
        }

        protected override void Render(HtmlTextWriter writer)
        {
            using (var sw = new StringWriter())
            using (var htw = new HtmlTextWriter(sw))
            {
                base.Render(htw);
                writer.Write(sw.ToString().RemoveHtmlWhitespace());
            }
        }

        public virtual string GetHtmlControl(IFieldControl field)
        {
            if (field == null)
                return string.Empty;

            var html = string.Empty;

            switch (field.Control)
            {
                case "TextBox":
                    html = string.Format("<input type=\"text\" id=\"{0}\" name=\"{0}\" class=\"form-control\"", field.Code);
                    if (!string.IsNullOrEmpty(field.Value))
                        html += string.Format(" value=\"{0}\"", field.Value);
                    if (field.IsRequired)
                        html += " required";
                    html += ">";
                    break;
                case "DropDownList":
                    break;
            }

            return html;
        }

        public string HtmlEncode(string value)
        {
            return value.HtmlEncode();
        }

        public string FormatDate(DateTime dateTime)
        {
            return FormatDateTime(dateTime, "yyyy-MM-dd");
        }

        public string FormatDateTime(DateTime dateTime)
        {
            return FormatDateTime(dateTime, "yyyy-MM-dd HH:mm:ss");
        }

        public string FormatDateTime(DateTime dateTime, string format)
        {
            return dateTime.ToString(format);
        }

        public string CurrentTabHead(string key, string value)
        {
            return Request.Get<string>(key) == value ? " class=\"current\"" : "";
        }

        public string CurrentTabContent(string key, string value)
        {
            return Request.Get<string>(key) == value ? " style=\"display:block;\"" : " style=\"display:none;\"";
        }

        protected void HandleException(Exception ex)
        {
            Response.Write(ex.ToString().HtmlEncode());
        }

        private static string adminPath;
        private static string sitePath;
        private static string siteUrl;

        public const string KeyPrefix = "known";
        //public static string RewriteConfig = string.Format("{0}static/config/rewrite.config", SitePath);

        public string AdminPath
        {
            get
            {
                if (string.IsNullOrEmpty(adminPath))
                {
                    adminPath = KConfig.GetSetting("admin_path", "");
                    if (!adminPath.EndsWith("/"))
                    {
                        adminPath += "/";
                    }
                }

                return adminPath;
            }
        }

        public string SitePath
        {
            get
            {
                if (string.IsNullOrEmpty(sitePath))
                {
                    sitePath = KConfig.GetSetting("site_path", "");
                    if (!sitePath.EndsWith("/"))
                    {
                        sitePath += "/";
                    }
                }

                return sitePath;
            }
        }

        public string SiteUrl
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
    }
}