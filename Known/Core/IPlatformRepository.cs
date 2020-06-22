using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core
{
    public interface IPlatformRepository
    {
        SysUser GetUser(Database db, string userName);
        List<MenuInfo> GetMenus(Database db);
        List<MenuInfo> GetUserMenus(Database db, string userName);
        string GetOrgName(Database db, string compNo, string orgNo);
    }

    public class MenuInfo
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public bool Checked { get; set; }
        public int Order { get; set; }
    }

    class PlatformRepository : IPlatformRepository
    {
        public SysUser GetUser(Database db, string userName)
        {
            var sql = "select * from SysUser where UserName=@userName";
            return db.QuerySingle<SysUser>(sql, new { userName });
        }

        public List<MenuInfo> GetMenus(Database db)
        {
            var sql = "select * from SysModule where Enabled=1 order by Sort";
            return db.QueryList<MenuInfo>(sql);
        }

        public List<MenuInfo> GetUserMenus(Database db, string userName)
        {
            var sql = @"
select * from SysModule 
where Enabled=1 and Id in (
  select a.ModuleId from SysRoleModule a,SysRole b,SysUserRole c,SysUser d
  where a.RoleId=b.Id and b.Id=c.RoleId and c.UserId=d.Id and d.UserName=@userName
  union 
  select a.ModuleId from SysUserModule a,SysUser b 
  where a.UserId=b.Id and b.UserName=@userName
)
order by Sort";
            return db.QueryList<MenuInfo>(sql, new { userName });
        }

        public string GetOrgName(Database db, string compNo, string orgNo)
        {
            var sql = "select Name from SysOrganization where CompNo=@compNo and Code=@orgNo";
            return db.Scalar<string>(sql, new { compNo, orgNo });
        }
    }
}
