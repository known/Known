using System;
using Known.Core;

namespace Known
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        public ModuleAttribute(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }

        public string Name { get; }
        public string Icon { get; }

        public ModuleInfo ToInfo()
        {
            return new ModuleInfo
            {
                Id = Utils.NewGuid,
                Name = Name,
                Icon = Icon
            };
        }
    }
}
