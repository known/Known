using AntDesign;

namespace Known.Helpers;

class RouteHelper
{
    private const string RouteId = "Route";

    internal static void AddRoute(Type type, List<RouteAttribute> routes)
    {
        var target = Constants.Route;
        if (!DataHelper.Routes.Exists(d => d.Id == RouteId))
        {
            var route = new MenuInfo { Id = RouteId, ParentId = "0", Name = "路由", Target = target, Icon = "share-alt", Url = "", Sort = 999 };
            DataHelper.Routes.Add(route);
        }

        if (routes.Count > 1)
        {
            var sub = new MenuInfo { Id = type.FullName, Name = type.Name, ParentId = RouteId, Target = target };
            DataHelper.Routes.Add(sub);
            foreach (var route in routes)
            {
                Config.RouteTypes[route.Template] = type;
                var info = new MenuInfo { Id = $"{type.FullName}_{route.Template}", ParentId = sub.Id, Target = target, Url = route.Template };
                SetRouteInfo(info, type);
                var table = AppData.CreateAutoPage(type);
                if (table != null)
                    info.Plugins.AddPlugin(table);
                DataHelper.Routes.Add(info);
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
            DataHelper.Routes.Add(info);
        }
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