using AntDesign;

namespace Known.Helpers;

class MenuHelper
{
    private const string RouteId = "KM_Route";
    private const string PluginId = "KM_Plugin";
    private const string RoleId = "KM_Role";

    internal static void AddParent()
    {
        var target = Constants.Route;
        AddParent(RoleId, "组件", "block", target, 997);
        AddParent(PluginId, "插件", "appstore-add", target, 998);
        AddParent(RouteId, "路由", "share-alt", target, 999);
    }

    internal static void AddMenu(Type type, RoleAttribute role, TabRoleAttribute tabRole, List<RouteAttribute> routes, object[] attributes)
    {
        var tabs = attributes.OfType<ReuseTabsPageAttribute>().FirstOrDefault();
        var plugin = attributes.OfType<PluginAttribute>().FirstOrDefault();
        var menu = attributes.OfType<MenuAttribute>().FirstOrDefault();

        var target = Constants.Route;
        var parentId = RouteId;
        if (role != null)
            parentId = RoleId;
        if (plugin != null)
            parentId = PluginId;

        if (routes != null && routes.Count > 1)
        {
            var sub = new MenuInfo { Id = type.FullName, Name = type.Name, ParentId = parentId, Target = target };
            DataHelper.Routes.Add(sub);
            foreach (var route in routes)
            {
                Config.RouteTypes[route.Template] = type;
                var info = new MenuInfo { Id = $"{type.FullName}_{route.Template}", ParentId = sub.Id, Target = target, Url = route.Template };
                SetRouteInfo(info, type, tabs, role, tabRole, plugin, menu);
                var table = CreateAutoPage(type);
                if (table != null)
                    info.Plugins.AddPlugin(table);
                DataHelper.Routes.Add(info);
            }
        }
        else
        {
            var url = string.Empty;
            if (routes != null && routes.Count > 0)
            {
                Config.RouteTypes[routes[0].Template] = type;
                url = routes[0].Template;
            }
            var info = new MenuInfo { Id = type.FullName, ParentId = parentId, Target = target, Url = url };
            SetRouteInfo(info, type, tabs, role, tabRole, plugin, menu);
            var table = CreateAutoPage(type);
            if (table != null)
            {
                info.Plugins.AddPlugin(table);
                //if (url == "/pms/works" || url == "/pms/abnormals")
                //{
                //    var tools = string.Join(",", table.Page?.Tools?.Select(d => $"{d.Id}={string.Join("-", d.Tabs ?? [])}"));
                //    Console.WriteLine($"{url}：{tools}");
                //}
            }
            DataHelper.Routes.Add(info);
        }
    }

    // 创建自动页面插件配置信息。
    internal static AutoPageInfo CreateAutoPage(Type pageType)
    {
        if (pageType?.BaseType?.IsGenericType == true)
        {
            var arguments = pageType.BaseType.GetGenericArguments();
            return AppDefaultData.CreateAutoPage(pageType, arguments[0]);
        }

        if (pageType?.BaseType?.BaseType?.IsGenericType == true)
        {
            var arguments = pageType.BaseType.BaseType.GetGenericArguments();
            return AppDefaultData.CreateAutoPage(pageType, arguments[0]);
        }

        return AppDefaultData.CreateAutoPage(pageType);
    }

    private static void AddParent(string id, string name, string icon, string target, int sort)
    {
        Language.DefaultDatas.Add(name);

        if (DataHelper.Routes.Any(d => d.Id == id))
            return;

        var route = new MenuInfo { Id = id, ParentId = "0", Name = name, Target = target, Icon = icon, Url = "", Sort = sort };
        DataHelper.Routes.Add(route);
    }

    private static void SetRouteInfo(MenuInfo info, Type type, ReuseTabsPageAttribute tabs, RoleAttribute role, TabRoleAttribute tabRole, PluginAttribute plugin, MenuAttribute menu)
    {
        info.Name = type.Name;
        info.Icon ??= "file";
        info.PageType = type;

        if (tabs != null)
        {
            info.Name = tabs.Title;
            Language.DefaultDatas.Add(info.Name);
            return;
        }

        if (role != null)
        {
            info.Name = role.Name;
            Language.DefaultDatas.Add(info.Name);
            return;
        }

        if (tabRole != null)
        {
            info.Name = tabRole.Name;
            info.ParentId = tabRole.Parent.FullName;
            Language.DefaultDatas.Add(info.Name);
            return;
        }

        if (plugin != null)
        {
            info.Name = plugin.Name;
            info.Icon = plugin.Icon;
            info.Sort = plugin.Sort;
            Language.DefaultDatas.Add(info.Name);
            return;
        }

        if (menu != null)
        {
            info.Type = nameof(MenuType.Link);
            info.Name = menu.Name;
            info.Icon = menu.Icon;
            info.ParentId = menu.Parent;
            info.Sort = menu.Sort;
            info.Target = nameof(LinkTarget.None);
            info.IsCode = true;
            Language.DefaultDatas.Add(info.Name);
        }
    }
}