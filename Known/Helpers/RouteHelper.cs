using AntDesign;

namespace Known.Helpers;

class RouteHelper
{
    private const string RouteId = "Route";
    private static List<MenuInfo> Routes { get; } = [];

    internal static void AddRoute(Type type, List<RouteAttribute> routes)
    {
        var target = Constants.Route;
        if (!Routes.Exists(d => d.Id == RouteId))
        {
            var route = new MenuInfo { Id = RouteId, ParentId = "0", Name = "路由", Target = target, Icon = "share-alt", Url = "", Sort = 999 };
            Routes.Add(route);
        }

        if (routes.Count > 1)
        {
            var sub = new MenuInfo { Id = type.FullName, Name = type.Name, ParentId = RouteId, Target = target };
            Routes.Add(sub);
            foreach (var route in routes)
            {
                Config.RouteTypes[route.Template] = type;
                var info = new MenuInfo { Id = $"{type.FullName}_{route.Template}", ParentId = sub.Id, Target = target, Url = route.Template };
                SetRouteInfo(info, type);
                var table = AppData.CreateAutoPage(type);
                if (table != null)
                    info.Plugins.AddPlugin(table);
                Routes.Add(info);
            }
        }
        else
        {
            Config.RouteTypes[routes[0].Template] = type;
            var info = new MenuInfo { Id = type.FullName, ParentId = RouteId, Target = target, Url = routes[0].Template };
            SetRouteInfo(info, type);
            var table = AppData.CreateAutoPage(type);
            if (table != null)
                info.Plugins.AddPlugin(table);
            Routes.Add(info);
        }
    }

    internal static void AddTo(List<MenuInfo> modules)
    {
        if (Routes.Count == 0)
            return;

        var items = Routes.Where(d => !modules.Exists(m => m.Id == d.Id || m.Url == d.Url)).ToList();
        var exists = Routes.Where(d => modules.Exists(m => m.Id == d.Id || m.Url == d.Url)).ToList();
        if (exists != null && exists.Count > 0)
        {
            foreach (var item in exists)
            {
                if (!item.IsCode)
                    continue;

                var info = modules.FirstOrDefault(m => m.Id == item.Id || m.Url == item.Url);
                info.Plugins = item.Plugins;
            }
        }

        if (items != null && items.Count > 0)
            modules.AddRange(items);
    }

    private static void SetRouteInfo(MenuInfo info, Type type)
    {
        info.Name = type.Name;
        info.Icon ??= "file";

        var tab = type.GetCustomAttribute<ReuseTabsPageAttribute>();
        if (tab != null)
        {
            info.Name = tab.Title;
            return;
        }

        var plugin = type.GetCustomAttribute<PluginAttribute>();
        if (plugin != null)
        {
            info.Name = plugin.Name;
            info.Icon = plugin.Icon;
            return;
        }

        var menu = type.GetCustomAttribute<MenuAttribute>();
        if (menu != null)
        {
            info.Type = nameof(MenuType.Link);
            info.Name = menu.Name;
            info.Icon = menu.Icon;
            info.ParentId = menu.Parent;
            info.Sort = menu.Sort;
            info.Target = nameof(LinkTarget.None);
            info.IsCode = true;
        }
    }
}