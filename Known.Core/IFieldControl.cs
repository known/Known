using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Known
{
    public interface IFieldControl
    {
        string Code { get; set; }
        string Control { get; set; }
        string CodeCategory { get; set; }
        string Name { get; set; }
        string Value { get; set; }
        bool IsRequired { get; set; }
    }
}
