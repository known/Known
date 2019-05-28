using Known.Core.Repositories;

namespace Known.Core.Services
{
    public class UserService : CoreServiceBase<IUserRepository>
    {
        public UserService(Context context) : base(context)
        {
        }
    }
}
