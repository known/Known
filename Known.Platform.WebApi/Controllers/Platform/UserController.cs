using System.Collections.Generic;
using System.Web.Http;
using Known.Platform.Services;
using Known.Platform.WebApi.Models;
using Known.Web;

namespace Known.Platform.WebApi.Controllers.Platform
{
    public class UserController : WebApiController
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

            return ApiResult.ToData(result.Data);
        }

        public ApiResult GetUser(string userName)
        {
            var user = Service.GetUser(userName);
            return ApiResult.ToData(user);
        }

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
                    Menu.SetSubModules(menus, item, menu);
                    index++;
                }
            }

            var codes = new Dictionary<string, object>();
            codes.Add("ViewType", Code.GetEnumCodes<ViewType>());

            return ApiResult.ToData(new { menus, codes });
        }

        public ApiResult GetCodes()
        {
            var codes = new Dictionary<string, object>();
            codes.Add("ViewType", Code.GetEnumCodes<ViewType>());
            return ApiResult.ToData(codes);
        }
    }
}