using Known.Data;

namespace Known.Core.Services
{
    public abstract class CoreServiceBase<TRepository> : ServiceBase<TRepository>
        where TRepository : IRepository
    {
        public CoreServiceBase(Context context) : base(context)
        {
        }
    }
}
