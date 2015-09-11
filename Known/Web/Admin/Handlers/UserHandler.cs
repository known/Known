using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Known.Models;

namespace Known.Web.Admin.Handlers
{
    public class UserHandler
    {
        public static void SaveUser(HttpContext context)
        {
            var user = new UserInfo
            {
                Id = context.Request.Get<string>("Id"),
                UserName = context.Request.Get<string>("UserName"),
                DisplayName = context.Request.Get<string>("DisplayName"),
                Email = context.Request.Get<string>("Email"),
                Roles = context.Request.Get<string>("Roles")
            };
            var result = AppContext.UserService.Save(user);
            context.Response.WriteJson(result);
        }

        public static void GetUser(HttpContext context)
        {
            var id = context.Request.Get<string>("Id");
            var user = AppContext.UserService.GetUser(id);
            context.Response.WriteJson(user);
        }

        public static void RemoveUser(HttpContext context)
        {
            var id = context.Request.Get<string>("Id");
            var result = AppContext.UserService.Remove(id);
            context.Response.WriteJson(result);
        }

        public static void ResetPassword(HttpContext context)
        {
            var id = context.Request.Get<string>("Id");
            var result = AppContext.UserService.ResetPassword(id);
            context.Response.WriteJson(result);
        }

        public static void ChangePassword(HttpContext context)
        {
            var id = context.Request.Get<string>("CurrentUserId");
            var oldPassword = context.Request.Get<string>("OldPassword");
            var newPassword = context.Request.Get<string>("NewPassword");
            var newPassword1 = context.Request.Get<string>("NewPassword1");
            var result = AppContext.UserService.ChangePassword(id, oldPassword, newPassword, newPassword1);
            context.Response.WriteJson(result);
        }
    }
}
