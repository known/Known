using System;
using System.Web;
using System.Web.UI;

namespace Known.Web.Extensions
{
    public static class PageExtension
    {
        public static void ShowMessage(this Page page, string message)
        {
            page.ShowMessage(message, null);
        }

        public static void ShowMessage(this Page page, string message, string attachScript)
        {
            var script = string.Format("alert('{0}');{1}", message, attachScript);
            page.RegisterStartupScript(script);
        }

        public static void RegisterBlockScript(this Page page, string script)
        {
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        public static void RegisterStartupScript(this Page page, string script)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        public static string HtmlEncode(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            value = HttpUtility.HtmlEncode(value);
            value = value.Replace("\r\n", "<br/>");
            value = value.Replace("\r", "<br/>");
            value = value.Replace("\n", "<br/>");
            return value;
        }
    }
}
