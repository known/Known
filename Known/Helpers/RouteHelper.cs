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
                var info = new MenuInfo { Id = $"{type.FullName}_{route.Template}", ParentId = sub.Id, Target = target, Url = route.Template, PageType = type };
                SetRouteInfo(info, type);
                var table = CreateAutoPage(type);
                if (table != null)
                    info.Plugins.AddPlugin(table);
                DataHelper.Routes.Add(info);
            }
        }
        else
        {
            Config.RouteTypes[routes[0].Template] = type;
            var info = new MenuInfo { Id = type.FullName, ParentId = RouteId, Target = target, Url = routes[0].Template, PageType = type };
            SetRouteInfo(info, type);
            var table = CreateAutoPage(type);
            if (table != null)
                info.Plugins.AddPlugin(table);
            DataHelper.Routes.Add(info);
        }
    }

    /// <summary>
    /// 创建自动页面插件配置信息。
    /// </summary>
    /// <param name="pageType">页面组件类型。</param>
    /// <returns>插件配置信息。</returns>
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