using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Known.Models
{
    public class MenuInfo
    {
        public string Id { get; set; }
        public string Parent { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Sequence { get; set; }
        public bool IsVisible { get; set; }

        public string GetFullName()
        {
            if (string.IsNullOrEmpty(Parent))
                return Name;

            return KCache.GetMenu(Parent).GetFullName() + "&nbsp;>&nbsp;" + Name;
        }
    }
}
