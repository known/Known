using System;

namespace Known
{
    public class BusinessBase
    {
        public BusinessBase(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Context Context { get; }

        protected T LoadBusiness<T>() where T : BusinessBase
        {
            return BusinessFactory.Create<T>(Context);
        }
    }
}
