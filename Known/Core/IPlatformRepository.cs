using System.Collections.Generic;

namespace Known.Core
{
    public interface IPlatformRepository
    {
        UserInfo GetUser(Database db, string userName);
        List<MenuInfo> GetUserMenus(Database db, string userName);
    }

    class PlatformRepository : IPlatformRepository
    {
        public UserInfo GetUser(Database db, string userName)
        {
            var sql = "select * from t_plt_users where user_name=@userName";
            //return db.QuerySingle<UserInfo>(sql, new { userName });
            return null;
        }

        public List<MenuInfo> GetUserMenus(Database db, string userName)
        {
            var sql = "select * from t_plt_users";
            //return db.QueryList<MenuInfo>(sql, new { userName });
            return null;
        }
    }
}
