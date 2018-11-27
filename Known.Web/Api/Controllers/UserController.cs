using System.Collections.Generic;
using System.Web.Http;
using Known.Platform;
using Known.Platform.Services;

namespace Known.Web.Api.Controllers
{
    public class UserController : BaseApiController
    {
        private UserService Service
        {
            get { return LoadService<UserService>(); }
        }

        [HttpGet]
        [AllowAnonymous]
        public ApiResult SignIn(string userName, string password)
        {
            var result = Service.SignIn(userName, password);
            if (!result.IsValid)
                return ApiResult.Error(result.Message);

            return ApiResult.Success(result.Data);
        }

        public ApiResult GetUser(string userName)
        {
            var user = Service.GetUser(userName);
            return ApiResult.Success(user);
        }

        #region GetUserMenus
        public ApiResult GetModules()
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
                    SetSubModules(menus, item, menu);
                    index++;
                }
            }

            var codes = new Dictionary<string, object>();
            codes.Add("ViewType", Code.GetEnumCodes<ViewType>());

            return ApiResult.Success(new { menus, codes });
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
        #endregion
    }
}