using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Known
{
    public class SettingInfo : IFieldControl
    {
        public string Code { get; set; }
        public string Control { get; set; }
        public string CodeCategory { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsRequired { get; set; }
    }
}
