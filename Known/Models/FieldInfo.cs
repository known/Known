using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Known.Models
{
    public class FieldInfo : IFieldControl
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Control { get; set; }
        public int MaxLength { get; set; }
        public string CodeCategory { get; set; }
        public string Value { get; set; }
        public bool IsRequired { get; set; }
        public bool IsSearch { get; set; }
        public bool IsList { get; set; }
        public bool IsEdit { get; set; }
        public bool IsView { get; set; }
        public int Sequence { get; set; }
        public bool IsVisible { get; set; }
    }
}
