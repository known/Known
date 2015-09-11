using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Known.Models;
using Known.Services;

namespace Known
{
    public class AppContext
    {
        public static UserInfo CurrentUser
        {
            get { return GetValue<UserInfo>("CurrentUser"); }
            set { SetValue("CurrentUser", value); }
        }

        public static MenuInfo CurrentMenu
        {
            get { return GetValue<MenuInfo>("CurrentMenu"); }
            set { SetValue("CurrentMenu", value); }
        }

        public static string AlertMessage
        {
            get { return GetValue<string>("AlertMessage"); }
            set { SetValue("AlertMessage", value); }
        }

        public static ISettingService SettingService
        {
            get { return LoadService<ISettingService>(); }
        }

        public static ICodeService CodeService
        {
            get { return LoadService<ICodeService>(); }
        }

        public static IMenuService MenuService
        {
            get { return LoadService<IMenuService>(); }
        }

        public static IRoleService RoleService
        {
            get { return LoadService<IRoleService>(); }
        }

        public static IUserService UserService
        {
            get { return LoadService<IUserService>(); }
        }

        public static T LoadService<T>()
        {
            return ServiceLoader.Load<T>();
        }

        public static T GetValue<T>(string key)
        {
            if (HttpContext.Current != null)
            {
                return (T)HttpContext.Current.Session[key];
            }
            return default(T);
        }

        public static void SetValue<T>(string key, T value)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session[key] = value;
            }
        }

        public static void RegisterServices()
        {
            ServiceLoader.Register<ISettingService, SettingService>();
            ServiceLoader.Register<ICodeService, CodeService>();
            ServiceLoader.Register<IMenuService, MenuService>();
            ServiceLoader.Register<IRoleService, RoleService>();
            ServiceLoader.Register<IUserService, UserService>();
            ServiceLoader.Register<IActionService, ActionService>();
            ServiceLoader.Register<IFieldService, FieldService>();
        }
    }
}
