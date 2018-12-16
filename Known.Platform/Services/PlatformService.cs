using Known.Data;

namespace Known.Platform.Services
{
    public abstract class PlatformService<TRepository> : ServiceBase<TRepository>
        where TRepository : IRepository
    {
        public PlatformService(Context context) : base(context)
        {
        }
    }
}
