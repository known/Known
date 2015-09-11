using Known;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Known.Models;

namespace System.Web.UI
{
    public static class ListControlExtension
    {
        public static void BindList(this ListControl control, string codeCategory, string emptyText)
        {
            control.BindList(codeCategory, emptyText, null);
        }

        public static void BindList(this ListControl control, string codeCategory, string emptyText, string defaultValue)
        {
            control.BindList(KCache.GetCodes(codeCategory), emptyText, defaultValue);
        }

        public static void BindList(this ListControl control, List<CodeInfo> codes, string emptyText, string defaultValue)
        {
            control.DataSource = codes;
            control.DataTextField = "Name";
            control.DataValueField = "Code";
            control.DataBind();

            if (!string.IsNullOrEmpty(emptyText))
            {
                control.Items.Insert(0, new ListItem(emptyText, ""));
            }

            if (string.IsNullOrEmpty(defaultValue))
            {
                control.SetSelectedValue(defaultValue);
            }
        }

        public static CodeInfo GetSelectedValue(this ListControl control)
        {
            var item = control.SelectedItem;
            if (item != null)
            {
                return new CodeInfo { Code = item.Value, Name = item.Text };
            }
            return null;
        }

        public static void SetSelectedValue(this ListControl control, CodeInfo value)
        {
            if (value == null)
                return;

            control.SetSelectedValue(value.Code);
        }

        public static void SetSelectedValue(this ListControl control, string value)
        {
            var item = control.Items.FindByValue(value);
            if (item != null)
            {
                item.Selected = true;
            }
        }
    }
}
