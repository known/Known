﻿using System.Collections.Generic;
using Known.Platform.Services;

namespace Known.Platform.Models
{
    public class Menu
    {
        public string id { get; set; }
        public string pid { get; set; }
        public string code { get; set; }
        public string text { get; set; }
        public string iconCls { get; set; }
        public string url { get; set; }
        public bool expanded { get; set; }
        public List<Menu> children { get; set; }

        public static List<Menu> GetUserMenus(UserService service)
        {
            var menus = new List<Menu>();
            var modules = service.GetModules();
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

            return menus;
        }

        public static List<Menu> GetMenus(ModuleService service)
        {
            var menus = new List<Menu>();
            var modules = service.GetModules(true);
            if (modules != null && modules.Count > 0)
            {
                foreach (var item in modules)
                {
                    var menu = Menu.GetMenu(item);
                    menu.expanded = item.ParentId == "-1" || item.ParentId == "0";
                    menus.Add(menu);
                }
            }

            return menus;
        }

        private static Menu GetMenu(Module module)
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

        private static void SetSubModules(List<Menu> menus, Module module, Menu menu)
        {
            if (module.Children == null || module.Children.Count == 0)
            {
                if (string.IsNullOrWhiteSpace(menu.url))
                {
                    menu.url = $"/frame?id={menu.id}";
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
}