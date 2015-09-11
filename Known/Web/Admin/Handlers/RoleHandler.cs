using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Known.Models;

namespace Known.Web.Admin.Handlers
{
    public class RoleHandler
    {
        public static void SaveRole(HttpContext context)
        {
            var role = new RoleInfo
            {
                Id = context.Request.Get<string>("Id"),
                Name = context.Request.Get<string>("Name"),
                Description = context.Request.Get<string>("Description")
            };
            var result = AppContext.RoleService.Save(role);
            context.Response.WriteJson(result);
        }

        public static void GetRole(HttpContext context)
        {
            var id = context.Request.Get<string>("Id");
            var role = AppContext.RoleService.GetRole(id);
            context.Response.WriteJson(role);
        }

        public static void RemoveRole(HttpContext context)
        {
            var id = context.Request.Get<string>("Id");
            var result = AppContext.RoleService.Remove(id);
            context.Response.WriteJson(result);
        }

        public static void SaveRight(HttpContext context)
        {
            var roleId = context.Request.Get<string>("RoleId");
            var menus = context.Request.Get<string>("Menus");
            var result = AppContext.RoleService.SaveRight(roleId, menus);
            context.Response.WriteJson(result);
        }

        public static void GetRight(HttpContext context)
        {
            var id = context.Request.Get<string>("Id");
            var role = AppContext.RoleService.GetRole(id);
            var menus = new List<MenuInfo>();
            foreach (var menu in KCache.GetMenus(null))
            {
                menus.Add(menu);
                foreach (var item in KCache.GetMenus(menu.Id))
                {
                    menus.Add(item);
                }
            }
            context.Response.WriteJson(new { Role = role, Menus = menus });
        }
    }
}
