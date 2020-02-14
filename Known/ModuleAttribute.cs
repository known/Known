using System;

namespace Known
{
    public class ModuleAttribute : Attribute
    {
        public ModuleAttribute(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }

        public ModuleAttribute(int order, string name, string icon)
        {
            Order = order;
            Name = name;
            Icon = icon;
        }

        public int Order { get; }

        public string Name { get; }

        public string Icon { get; }

        public string Code { get; internal set; }

        public string Parent { get; internal set; }

        public string Url { get; internal set; }
    }
}
