using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core.Datas
{
    public interface IPlatformRepository
    {
        SysUser GetUser(Database db, string userName);
        List<MenuInfo> GetUserMenus(Database db, string userName, string parentId);
    }

    class PlatformRepository : IPlatformRepository
    {
        public SysUser GetUser(Database db, string userName)
        {
            var sql = "select * from SysUser where UserName=@userName";
            return db.QuerySingle<SysUser>(sql, new { userName });
        }

        public List<MenuInfo> GetUserMenus(Database db, string userName, string parentId)
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                var sql = "select * from SysModule where ParentId=''";
                return db.QueryList<MenuInfo>(sql);
            }
            else
            {
                var sql = $@"
with cte_parent(Id)
as
(
    select a.Id from SysModule a 
	where a.Enabled=1 and a.ParentId='{parentId}' 
	union all 
	select a.Id from SysModule a 
    inner join cte_parent b on a.ParentId=b.Id  
	where a.Enabled=1
)
select *
from SysModule a 
where a.Id in (select Id from cte_parent) 
order by a.Sort";
                return db.QueryList<MenuInfo>(sql, new { parentId });
            }
        }
    }
}
