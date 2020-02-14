using System;
using System.Collections.Generic;

namespace Known.Web.Mvc
{
    public class ControllerInfo
    {
        public ControllerInfo(Type type)
        {
            Type = type;
            Name = type.Name.Replace("Controller", "");
            Actions = new List<ActionInfo>();
        }

        public string Name { get; }
        public Type Type { get; }
        public List<ActionInfo> Actions { get; }
    }
}
