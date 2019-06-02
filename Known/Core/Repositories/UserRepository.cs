using Known.Data;

namespace Known.Core.Repositories
{
    /// <summary>
    /// 系统用户数据仓库接口。
    /// </summary>
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
