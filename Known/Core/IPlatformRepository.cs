using System.Collections.Generic;
using System.Data.Common;

namespace Known.Core
{
    public interface IPlatformRepository
    {
        UserInfo GetUser(DbConnection conn, string userName);
        List<MenuInfo> GetUserMenus(DbConnection conn, string userName);
    }

    class PlatformRepository : IPlatformRepository
    {
        public UserInfo GetUser(DbConnection conn, string userName)
        {
            //var sql = "select * from t_users where user_name=@userName";
            return null;
        }

        public List<MenuInfo> GetUserMenus(DbConnection conn, string userName)
        {
            throw new System.NotImplementedException();
        }
    }
}
