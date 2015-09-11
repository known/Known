using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace System.Web.UI
{
    public static class ControlExtension
    {
        public static T FindControl<T>(this Control control, string id) where T : Control
        {
            return control.FindControl(id) as T;
        }

        public static List<T> FindControls<T>(this Control control) where T : Control
        {
            var list = new List<T>();
            control.Controls.ForEach<Control>(c =>
            {
                if (c is T)
                {
                    list.Add(c as T);
                }
            });
            return list;
        }

        public static string GetValue(this Control control)
        {
            return control.GetValue(false);
        }

        public static string GetValue(this Control control, bool trim)
        {
            if (control is ITextControl)
            {
                var text = (control as ITextControl).Text;
                return trim ? text.Trim() : text;
            }
            if (control is ListControl)
            {
                return (control as ListControl).SelectedValue;
            }
            if (control is ICheckBoxControl)
            {
                return (control as ICheckBoxControl).Checked ? "Y" : "N";
            }
            if (control is HiddenField)
            {
                var value = (control as HiddenField).Value;
                return trim ? value.Trim() : value;
            }
            return string.Empty;
        }

        public static void SetValue(this Control control, object value)
        {
            string valueString = string.Empty;
            if (value != null)
            {
                valueString = value.ToString();
            }

            if (control is ITextControl)
            {
                (control as ITextControl).Text = valueString;
            }
            if (control is ListControl)
            {
                (control as ListControl).SetSelectedValue(valueString);
            }
            if (control is ICheckBoxControl)
            {
                (control as ICheckBoxControl).Checked = valueString == "Y";
            }
            if (control is HiddenField)
            {
                (control as HiddenField).Value = valueString;
            }
        }

        public static void ClearControls(this Control container)
        {
            foreach (var control in container.Controls)
            {
                if (control is ITextControl)
                {
                    ((ITextControl)control).Text = string.Empty;
                }
                else if (control is ListControl)
                {
                    ((ListControl)control).SelectedIndex = -1;
                }
                else if (control is CheckBox)
                {
                    ((CheckBox)control).Checked = false;
                }
                //else if (control is RadioButtonList)
                //{
                //    ((RadioButtonList)control).ClearSelection();
                //}
            }
        }

        public static void EnableControls(this Control container, bool enable)
        {
            foreach (var control in container.Controls)
            {
                if (control is WebControl)
                {
                    ((WebControl)control).Enabled = enable;
                }
            }
        }

        public static void FillRow(this Control container, DataRow row)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                foreach (Control control in container.Controls)
                {
                    if (column.ColumnName.Replace("_", "").Equals(GetControlId(control.ID), StringComparison.OrdinalIgnoreCase))
                    {
                        control.SetValue(row[column.ColumnName]);
                    }
                }
            }
        }

        public static void UpdateRow(this Control container, DataRow row)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                foreach (Control control in container.Controls)
                {
                    if (column.ColumnName.Replace("_", "").Equals(GetControlId(control.ID), StringComparison.OrdinalIgnoreCase))
                    {
                        row[column.ColumnName] = control.GetValue();
                    }
                }
            }
        }

        private static string GetControlId(string controlId)
        {
            if (!string.IsNullOrEmpty(controlId))
            {
                return controlId.Replace("_", "")
                                .Replace("txt", "")
                                .Replace("ddl", "")
                                .Replace("cbl", "")
                                .Replace("rbl", "")
                                .Replace("cb", "")
                                .Replace("rb", "");
            }

            return string.Empty;
        }
    }
}
