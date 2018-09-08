using System;
using Known.Data;
using Known.Log;

namespace Known
{
    public abstract class BusinessBase
    {
        public BusinessBase(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Context Context { get; }

        public Database Database
        {
            get { return Context.Database; }
        }

        public ILogger Logger
        {
            get { return Context.Logger; }
        }

        protected T LoadBusiness<T>() where T : BusinessBase
        {
            return BusinessFactory.Create<T>(Context);
        }
    }
}
