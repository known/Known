using System.Collections.Generic;
using Known.Data;

namespace Known.Platform.Repositories
{
    public interface IUserRepository : IRepository
    {
        User GetUser(string userName);
        List<Module> GetUserModules(string userName);
    }

    internal class UserRepository : DbRepository, IUserRepository
    {
        public User GetUser(string userName)
        {
            var sql = "select * from t_plt_users where user_name=@user_name";
            return Database.Query<User>(sql, new { user_name = userName });
        }

        public List<Module> GetUserModules(string userName)
        {
            var sql = "select * from t_plt_modules";
            return Database.QueryList<Module>(sql);
        }
    }
}
