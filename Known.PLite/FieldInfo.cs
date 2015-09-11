using Known.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Known.PLite
{
    public class FieldInfo
    {
        public string Id { get; set; }
        public string Module { get; set; }
        public ControlType ControlType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int MaxLength { get; set; }
        public string CodeCategory { get; set; }
        public string DefaultValue { get; set; }
        public bool IsSearch { get; set; }
        public bool IsList { get; set; }
        public bool IsEdit { get; set; }
        public bool IsView { get; set; }
        public int Sequence { get; set; }
        public bool IsVisible { get; set; }

        private static List<FieldInfo> Fields
        {
            get
            {
                var key = KConfig.KeyPrefix + "fieldinfos";
                var fields = KCache.Get<List<FieldInfo>>(key);
                if (fields == null)
                {
                    var service = ServiceLoader.Load<IPrototypeService>();
                    fields = service.GetFields();
                    KCache.Insert(key, fields);
                }
                return fields;
            }
        }

        public Control GetControl()
        {
            Control control = null;
            switch (ControlType)
            {
                case ControlType.TextBox:
                    break;
                case ControlType.TextArea:
                    break;
                case ControlType.CheckBox:
                    break;
                case ControlType.RadioButton:
                    break;
                case ControlType.DropDownList:
                    break;
                case ControlType.CheckBoxList:
                    break;
                case ControlType.RadioButtonList:
                    break;
                case ControlType.RichText:
                    break;
                case ControlType.Date:
                    break;
                case ControlType.DateRange:
                    break;
                case ControlType.Custom:
                    break;
                default:
                    break;
            }
            return control;
        }

        public static List<FieldInfo> GetFields(string module)
        {
            return Fields.Where(a => a.Module == module).OrderBy(a => a.Sequence).ToList();
        }
    }
}
