using System.Collections.Generic;
using System.Linq;

namespace Known.Core
{
    /// <summary>
    /// 系统菜单类。
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// 取得或设置ID。
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 取得或设置上级ID。
        /// </summary>
        public string pid { get; set; }

        /// <summary>
        /// 取得或设置编码。
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 取得或设置显示文本。
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 取得或设置图标。
        /// </summary>
        public string iconCls { get; set; }

        /// <summary>
        /// 取得或设置地址。
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 取得或设置是否展开。
        /// </summary>
        public bool expanded { get; set; }

        /// <summary>
        /// 取得或设置子菜单列表。
        /// </summary>
        public List<Menu> children { get; set; }

        /// <summary>
        /// 取得或设置模块按钮集合。
        /// </summary>
        public List<object> buttons { get; set; }

        /// <summary>
        /// 取得或设置模块列表页面栏位集合。
        /// </summary>
        public List<string> columns { get; set; }

        /// <summary>
        /// 获取所有模块信息列表。
        /// </summary>
        /// <param name="service">平台服务对象。</param>
        /// <returns>模块信息列表。</returns>
        public static List<Menu> GetTreeMenus(PlatformService service)
        {
            var app = AppInfo.Instance;
            var menus = new List<Menu>
            {
                new Menu
                {
                    id = "0",
                    pid = "-1",
                    code = app.Id,
                    text = app.Name,
                    expanded = true
                }
            };

            var modules = service.GetModules();
            if (modules != null && modules.Count > 0)
            {
                foreach (var item in modules)
                {
                    var menu = GetMenu(item);
                    menu.expanded = item.ParentId == "-1" || item.ParentId == "0";
                    menus.Add(menu);
                }
            }

            return menus;
        }

        /// <summary>
        /// 获取指定用户权限的模块信息列表。
        /// </summary>
        /// <param name="service">平台服务对象。</param>
        /// <param name="userName">登录用户名。</param>
        /// <returns>模块信息列表。</returns>
        public static List<Menu> GetUserMenus(PlatformService service, string userName)
        {
            var menus = new List<Menu>();

            var modules = service.GetUserModules(userName);
            if (modules != null && modules.Count > 0)
            {
                var index = 0;
                foreach (var item in modules)
                {
                    var menu = GetMenu(item);
                    menu.expanded = index == 0;
                    menus.Add(menu);
                    index++;
                }
            }

            return menus;
        }

        private static Menu GetMenu(ModuleAttribute module)
        {
            var menu = new Menu
            {
                id = module.Code,
                pid = module.Parent ?? "0",
                code = module.Code,
                text = module.Name,
                iconCls = module.Icon,
                url = module.Url,
                columns = module.Columns
            };
            if (module.Buttons != null && module.Buttons.Count > 0)
            {
                menu.buttons = new List<object>();
                foreach (var item in module.Buttons.OrderBy(b => b.Order))
                {
                    menu.buttons.Add(new
                    {
                        id = item.Id,
                        text = item.Name,
                        iconCls = item.Icon,
                        url = item.Url,
                        sort = item.Order,
                        form = item.IsForm
                    });
                }
            }

            return menu;
        }

        private static Menu GetMenu(ModuleInfo module)
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

        private static void SetSubModules(List<Menu> menus, ModuleInfo module, Menu menu)
        {
            if (module.Children == null || module.Children.Count == 0)
                return;

            menu.children = new List<Menu>();
            foreach (var item in module.Children)
            {
                var menu1 = GetMenu(item);
                menu.children.Add(menu1);
                SetSubModules(menus, item, menu1);
            }
        }
    }
}