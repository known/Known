using Known.Data;

namespace Known.Core.Repositories
{
    public interface IUserRepository : IRepository
    {
    }

    internal class UserRepository : DbRepository, IUserRepository
    {
        public UserRepository(Database database) : base(database)
        {
        }
    }
}
