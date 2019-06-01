using Known.Data;

namespace Known.Core.Services
{
    abstract class AdminService<TRepository> : ServiceBase<TRepository>
        where TRepository : IRepository
    {
        public AdminService(Context context) : base(context)
        {
        }
    }
}
