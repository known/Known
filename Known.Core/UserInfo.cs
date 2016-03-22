using Known.Extensions;
using Known.Services;
using System.Collections.Generic;

namespace Known
{
    public class UserInfo
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }

        public List<MenuInfo> GetMenus(string parent)
        {
            var menuCodes = new List<string>();
            var roles = AppContext.LoadService<IRoleService>().GetRoles(Roles);
            roles.ForEach(r => menuCodes.AddRange(r.Menus.Split(',')));

            var menus = new List<MenuInfo>();
            //foreach (var item in KCache.GetMenus(parent))
            //{
            //    if (menuCodes.Contains(item.Id) || menuCodes.Contains("ALL"))
            //    {
            //        menus.Add(item);
            //    }
            //}
            return menus;
        }

        public static UserInfo CheckUser(string userName, string password, out string message)
        {
            var user = AppContext.LoadService<IUserService>().GetUserByUserName(userName);
            if (user == null)
            {
                message = "用户不存在！";
                return null;
            }

            if (user.Password != password.ToMd5())
            {
                message = "用户密码不正确！";
                return null;
            }

            message = string.Empty;
            return user;
        }
    }
}
