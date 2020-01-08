using System;

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

        public virtual void Init(Context context) { }

        protected PageView Page(int order, string code, string name, string icon)
        {
            return new PageView(Type, order, code, name, icon);
        }
    }
}
