using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core
{
    public interface IPlatformRepository
    {
        SysUser GetUser(Database db, string userName);
        List<MenuInfo> GetUserMenus(Database db, string userName);
    }

    public class MenuInfo
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }

        public object ToTree()
        {
            return new
            {
                id = Id,
                pid = ParentId,
                code = Code,
                title = Name,
                icon = Icon,
                url = Url
            };
        }

        public static object ToTree(SysModule module)
        {
            return new
            {
                id = module.Id,
                pid = module.ParentId,
                title = module.Name,
                icon = module.Icon,
                module
            };
        }
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
