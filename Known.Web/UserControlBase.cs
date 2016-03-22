using Known.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Known.Web
{
    public class UserControlBase : UserControl
    {
        public string VirtualPath
        {
            //get { return KConfig.AdminPath; }
            get { return ""; }
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

        public virtual string GetGridButton(string text, string icon, string onclick)
        {
            return GetGridButton(text, icon, onclick, false);
        }

        public virtual string GetGridButton(string text, string icon, string onclick, bool disabled)
        {
            var format = "<span class=\"{0}\"{1}><i class=\"{2}\"></i>&nbsp;{3}</span>";
            var css = disabled ? "btn-disabled" : "btn-grid";
            var click = disabled ? "" : string.Format(" onclick=\"{0}\"", onclick);
            return string.Format(format, css, click, icon, text);
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
    }
}
