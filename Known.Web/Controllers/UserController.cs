using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using Known.Extensions;
using Known.Platform;
using Known.Platform.Services;
using Known.Web.Filters;

namespace Known.Web.Controllers
{
    public class UserController : BaseController
    {
        private UserService Service
        {
            get { return LoadService<UserService>(); }
        }

        public ActionResult SignIn(string userName, string password, string backUrl)
        {
            userName = userName.ToLower();
            var result = Service.SignIn(userName, password);
            if (!result.IsValid)
                return ErrorResult(result.Message);

            FormsAuthentication.SetAuthCookie(userName, true);
            CurrentUser = result.Data;

            if (string.IsNullOrEmpty(backUrl))
                backUrl = FormsAuthentication.DefaultUrl;

            return SuccessResult("登录成功，正在跳转页面......", backUrl);
        }

        [LoginAuthorize]
        public void SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
        }

        [LoginAuthorize]
        public ActionResult GetModules()
        {
            var menus = new List<Menu>();
            var modules = Service.GetUserModules();
            if (modules != null && modules.Count > 0)
            {
                var index = 0;
                foreach (var item in modules)
                {
                    var menu = Menu.GetMenu(item);
                    menu.expanded = index == 0;
                    menus.Add(menu);
                    Menu.SetSubModules(menus, item, menu);
                    index++;
                }
            }

            var codes = new Dictionary<string, object>();
            codes.Add("ViewType", Code.GetEnumCodes<Known.Platform.ViewType>());

            return JsonResult(new { menus, codes });
        }

        [LoginAuthorize]
        public ActionResult GetCodes()
        {
            var codes = new Dictionary<string, object>();
            codes.Add("ViewType", Code.GetEnumCodes<Known.Platform.ViewType>());
            return JsonResult(codes);
        }
    }

    class Menu
    {
        public string id { get; set; }
        public string pid { get; set; }
        public string code { get; set; }
        public string text { get; set; }
        public string iconCls { get; set; }
        public string url { get; set; }
        public bool expanded { get; set; }
        public List<Menu> children { get; set; }

        public static Menu GetMenu(Module module)
        {
            return new Menu
            {
                id = module.Id,
                pid = module.ParentId,
                code = module.Code,
                text = module.Name,
                iconCls = module.Icon,
                url = module.Url
            };
        }

        public static void SetSubModules(List<Menu> menus, Module module, Menu menu)
        {
            if (module.Children == null || module.Children.Count == 0)
            {
                if (string.IsNullOrWhiteSpace(menu.url))
                {
                    menu.url = $"/frame?mid={menu.id}";
                }
                return;
            }

            menu.children = new List<Menu>();
            foreach (var item in module.Children)
            {
                var menu1 = GetMenu(item);
                menu.children.Add(menu1);
                SetSubModules(menus, item, menu1);
            }
        }
    }

    class Code
    {
        public string id { get; set; }
        public string text { get; set; }

        public static List<Code> GetEnumCodes<T>()
        {
            var codes = new List<Code>();
            var enumType = typeof(T);
            var values = Enum.GetValues(enumType);

            foreach (Enum value in values)
            {
                var id = Convert.ToInt32(value).ToString();
                var text = value.GetDescription();
                if (string.IsNullOrWhiteSpace(text))
                {
                    text = Enum.GetName(enumType, value);
                }

                codes.Add(new Code { id = id, text = text });
            }

            return codes;
        }

        public static List<Code> GetStringCodes(params string[] values)
        {
            var codes = new List<Code>();

            foreach (var item in values)
            {
                codes.Add(new Code { id = item, text = item });
            }

            return codes;
        }
    }
}