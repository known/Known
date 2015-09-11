using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Known.Models
{
    public class CodeInfo
    {
        public string Category { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Sequence { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
