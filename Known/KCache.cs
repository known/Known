using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Known.Models;

namespace Known
{
    public class KCache
    {
        private static List<CodeInfo> GetCodes()
        {
            var key = "codeinfos";
            var codes = KCache.Get<List<CodeInfo>>(key);
            if (codes == null)
            {
                codes = AppContext.CodeService.GetCodes();
                KCache.Insert(key, codes);
            }
            return codes;
        }

        public static List<CodeInfo> GetCodes(string category)
        {
            return GetCodes().Where(c => c.Category == category).OrderBy(c => c.Sequence).ToList();
        }

        public static CodeInfo GetCode(string category, string code)
        {
            return GetCodes().FirstOrDefault(c => c.Category == category && c.Code == code);
        }

        private static List<MenuInfo> GetMenus()
        {
            var key = "menuinfos";
            var menus = KCache.Get<List<MenuInfo>>(key);
            if (menus == null)
            {
                menus = AppContext.MenuService.GetMenus();
                KCache.Insert(key, menus);
            }
            return menus;
        }

        public static List<MenuInfo> GetMenus(string parent)
        {
            return GetMenus().Where(m => m.Parent == parent).OrderBy(m => m.Sequence).ToList();
        }

        public static MenuInfo GetMenu(string id)
        {
            return GetMenus().FirstOrDefault(m => m.Id == id) ?? new MenuInfo { Name = "仪表盘" };
        }

        public static MenuInfo GetCurrentMenu(string url)
        {
            return GetMenus().FirstOrDefault(m => m.Url == url) ?? new MenuInfo { Name = "仪表盘" };
        }

        public static T Get<T>(string key)
        {
            var strKey = KConfig.KeyPrefix + key;
            if (HttpContext.Current != null)
            {
                return (T)HttpContext.Current.Cache.Get(strKey);
            }
            return default(T);
        }

        public static void Insert<T>(string key, T value)
        {
            var strKey = KConfig.KeyPrefix + key;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Cache.Insert(strKey, value);
            }
        }
    }
}
