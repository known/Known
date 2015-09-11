using Known.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Known.PLite
{
    public class BizColumn
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public int MaxLength { get; set; }
        public bool IsNullable { get; set; }
        public ControlType Control { get; set; }
        public bool IsList { get; set; }
        public bool IsSearch { get; set; }
    }
}
