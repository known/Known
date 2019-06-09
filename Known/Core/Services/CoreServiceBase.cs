using Known.Data;

namespace Known.Core
{
    abstract class CoreServiceBase<TRepository> : ServiceBase<TRepository>
        where TRepository : IRepository
    {
        public CoreServiceBase(Context context) : base(context)
        {
        }
    }
}
