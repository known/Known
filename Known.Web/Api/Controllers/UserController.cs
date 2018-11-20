using System.Collections.Generic;
using System.Web.Http;
using Known.Platform;
using Known.Platform.Business;

namespace Known.Web.Api.Controllers
{
    public class UserController : BaseApiController
    {
        private UserBusiness Business
        {
            get { return LoadBusiness<UserBusiness>(); }
        }

        [HttpGet]
        [AllowAnonymous]
        public ApiResult SignIn(string userName, string password)
        {
            var result = Business.SignIn(userName, password);
            if (!result.IsValid)
                return ApiResult.Error(result.Message);

            return ApiResult.Success(result.Data);
        }

        [HttpGet]
        public ApiResult GetUser(string userName)
        {
            var user = Business.GetUser(userName);
            return ApiResult.Success(user);
        }

        #region GetUserMenus
        [HttpGet]
        public ApiResult GetModules()
        {
            var menus = new List<Menu>();
            var modules = Business.GetUserModules();
            if (modules != null && modules.Count > 0)
            {
                var index = 0;
                foreach (var item in modules)
                {
                    var menu = Menu.GetMenu(item);
                    menu.expanded = index == 0;
                    menus.Add(menu);
                    SetSubModules(menus, item, menu);
                    index++;
                }
            }

            return ApiResult.Success(menus);
        }

        private void SetSubModules(List<Menu> menus, Module module, Menu menu)
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
                var menu1 = Menu.GetMenu(item);
                menu.children.Add(menu1);
                SetSubModules(menus, item, menu1);
            }
        }

        class Menu
        {
            public string id { get; set; }
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
                    code = module.Code,
                    text = module.Name,
                    iconCls = module.Icon,
                    url = module.Url
                };
            }
        }
        #endregion
    }
}