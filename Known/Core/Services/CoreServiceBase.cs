using Known.Data;

namespace Known.Core.Services
{
    abstract class CoreServiceBase<TRepository> : ServiceBase<TRepository>
        where TRepository : IRepository
    {
        public CoreServiceBase(Context context) : base(context)
        {
        }
    }
}
