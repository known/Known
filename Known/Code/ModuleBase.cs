using System;
using Known.Core;

namespace Known
{
    public abstract class ModuleBase
    {
        public abstract string Name { get; }
        public abstract string Icon { get; }

        public Type Type
        {
            get { return GetType(); }
        }

        public ModuleInfo ToInfo()
        {
            return new ModuleInfo
            {
                Id = Utils.NewGuid,
                Name = Name,
                Icon = Icon
            };
        }

        protected PageView Page(string code, string name)
        {
            return new PageView(Type, code, name);
        }
    }
}
