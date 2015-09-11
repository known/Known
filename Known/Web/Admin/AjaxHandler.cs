using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

using Known.Web.Admin.Handlers;

namespace Known.Web.Admin
{
    public class AjaxHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            if (AppContext.CurrentUser == null)
            {
                context.Response.WriteError("登录超时，请<a href=\"{0}Login.aspx\">重新登陆</a>！", KConfig.AdminPath);
            }

            var action = context.Request.Get<string>("act");
            switch (action)
            {
                case "load_page": LoadPage(context); break;
                case "save_setting": SaveSetting(context); break;
                case "save_role": RoleHandler.SaveRole(context); break;
                case "get_role": RoleHandler.GetRole(context); break;
                case "remove_role": RoleHandler.RemoveRole(context); break;
                case "save_right": RoleHandler.SaveRight(context); break;
                case "get_right": RoleHandler.GetRight(context); break;
                case "save_user": UserHandler.SaveUser(context); break;
                case "get_user": UserHandler.GetUser(context); break;
                case "remove_user": UserHandler.RemoveUser(context); break;
                case "reset_password": UserHandler.ResetPassword(context); break;
                case "change_password": UserHandler.ChangePassword(context); break;
            }
        }

        private static void SaveSetting(HttpContext context)
        {
            var settings = KSettings.GetSettings();
            foreach (var item in settings)
            {
                item.Value = context.Request.Get<string>(item.Code);
            }
            var result = KSettings.UpdateSettings(settings);
            context.Response.WriteJson(result);
        }

        private static void LoadPage(HttpContext context)
        {
            var id = context.Request.Get<string>("Id");
            var menu = KCache.GetMenu(id);
            if (menu == null || string.IsNullOrEmpty(menu.Url))
            {
                context.Response.WriteError("<strong>页面不存在！</strong>");
            }

            var path = Utils.GetAdminUrl(menu.Url);
            var filePath = context.Server.MapPath(path);
            if (!File.Exists(filePath))
            {
                context.Response.WriteError("<strong>页面不存在！</strong><br/>路径：{0}", path);
            }

            if (menu.Url.Contains(".ascx"))
            {
                var page = new Page();
                var control = page.LoadControl(path);
                page.Controls.Add(control);
                using (var writer = new StringWriter())
                {
                    context.Server.Execute(page, writer, false);
                    var html = writer.ToString().RemoveHtmlWhitespace();
                    context.Response.WriteText(html);
                }
            }

            using (var writer = new StringWriter())
            {
                context.Server.Execute(path, writer, false);
                var html = writer.ToString().RemoveHtmlWhitespace();
                context.Response.WriteText(html);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
