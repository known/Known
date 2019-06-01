using Known.Core.Repositories;

namespace Known.Core.Services
{
    class UserService : CoreServiceBase<IUserRepository>
    {
        public UserService(Context context) : base(context)
        {
        }
    }
}
