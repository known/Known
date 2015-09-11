using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Known.Models
{
    public class ActionInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public bool IsVisible { get; set; }
    }
}
