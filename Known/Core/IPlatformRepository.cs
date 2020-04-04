using System.Collections.Generic;

namespace Known.Core
{
    public interface IPlatformRepository
    {
        UserInfo GetUser(Database db, string userName);
        List<MenuInfo> GetUserMenus(Database db, string userName, string parentId);
    }

    class PlatformRepository : IPlatformRepository
    {
        public UserInfo GetUser(Database db, string userName)
        {
            //var sql = "select * from t_plt_users where user_name=@userName";
            //return db.QuerySingle<UserInfo>(sql, new { userName });
            return null;
        }

        public List<MenuInfo> GetUserMenus(Database db, string userName, string parentId)
        {
            var menus = new List<MenuInfo>();
            if (string.IsNullOrWhiteSpace(parentId))
            {
                menus.Add(new MenuInfo { Id = "1", ParentId = "", Name = "开发框架", Icon = "&#xe614;" });
                return menus;
            }

            menus.Add(new MenuInfo { Id = "11", ParentId = "1", Name = "系统管理", Icon = "&#xe614;" });
            menus.Add(new MenuInfo { Id = "111", ParentId = "11", Name = "模块管理", Icon = "", Url = "/System/ModuleView" });
            menus.Add(new MenuInfo { Id = "112", ParentId = "11", Name = "角色管理", Icon = "", Url = "/System/RoleView" });
            menus.Add(new MenuInfo { Id = "113", ParentId = "11", Name = "用户管理", Icon = "", Url = "/System/UserView" });
            return menus;

            //var sql = "select * from t_plt_users";
            //return db.QueryList<MenuInfo>(sql, new { userName });
        }
    }
}
