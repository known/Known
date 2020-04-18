using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core.Datas
{
    public interface IPlatformRepository
    {
        SysUser GetUser(Database db, string userName);
        List<MenuInfo> GetUserMenus(Database db, string userName);
    }

    class PlatformRepository : IPlatformRepository
    {
        public SysUser GetUser(Database db, string userName)
        {
            var sql = "select * from SysUser where UserName=@userName";
            return db.QuerySingle<SysUser>(sql, new { userName });
        }

        public List<MenuInfo> GetUserMenus(Database db, string userName)
        {
            var sql = "select * from SysModule where Enabled=1 order by Sort";
            return db.QueryList<MenuInfo>(sql);
        }
    }
}
