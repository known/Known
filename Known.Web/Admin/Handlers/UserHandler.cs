using Known.Services;
using Known.Web.Extensions;
using System.Web;

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
            var result = AppContext.LoadService<IUserService>().Save(user);
            context.Response.WriteJson(result);
        }

        public static void GetUser(HttpContext context)
        {
            var id = context.Request.Get<string>("Id");
            var user = AppContext.LoadService<IUserService>().GetUser(id);
            context.Response.WriteJson(user);
        }

        public static void RemoveUser(HttpContext context)
        {
            var id = context.Request.Get<string>("Id");
            var result = AppContext.LoadService<IUserService>().Remove(id);
            context.Response.WriteJson(result);
        }

        public static void ResetPassword(HttpContext context)
        {
            var id = context.Request.Get<string>("Id");
            var result = AppContext.LoadService<IUserService>().ResetPassword(id);
            context.Response.WriteJson(result);
        }

        public static void ChangePassword(HttpContext context)
        {
            var id = context.Request.Get<string>("CurrentUserId");
            var oldPassword = context.Request.Get<string>("OldPassword");
            var newPassword = context.Request.Get<string>("NewPassword");
            var newPassword1 = context.Request.Get<string>("NewPassword1");
            var result = AppContext.LoadService<IUserService>().ChangePassword(id, oldPassword, newPassword, newPassword1);
            context.Response.WriteJson(result);
        }
    }
}
