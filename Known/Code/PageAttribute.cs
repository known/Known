using System;
using Known.Core;

namespace Known
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PageAttribute : Attribute
    {
        public PageAttribute(int order, string name, string icon)
        {
            Order = order;
            Name = name;
            Icon = icon;
        }

        public int Order { get; }
        public string Name { get; }
        public string Icon { get; }

        public ModuleInfo ToInfo()
        {
            return new ModuleInfo
            {
                Id = Utils.NewGuid,
                Name = Name,
                Icon = Icon,
                Sort = Order
            };
        }
    }
}
