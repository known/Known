using System;
using System.Collections.Generic;
using System.Reflection;
using Known.Core;

namespace Known.Web.Mvc
{
    public class ControllerInfo
    {
        private ControllerInfo(Type type)
        {
            Type = type;
            Name = type.Name.Replace("Controller", "");
            Actions = new List<ActionInfo>();
        }

        public string Name { get; }
        public Type Type { get; }
        public List<ActionInfo> Actions { get; }

        internal static ControllerInfo Create(Type type)
        {
            var info = new ControllerInfo(type);

            var attr = type.GetCustomAttribute<ModuleAttribute>();
            if (attr != null)
            {
                attr.Code = info.Name;
                AppInfo.Instance.AddModule(attr);
            }

            return info;
        }
    }
}
