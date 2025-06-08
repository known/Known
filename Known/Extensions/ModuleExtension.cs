namespace Known.Extensions;

/// <summary>
/// 模块信息扩展类。
/// </summary>
public static class ModuleExtension
{
    /// <summary>
    /// 添加代码配置的模块列表。
    /// </summary>
    /// <param name="modules">模块信息列表。</param>
    /// <param name="lists">要添加的信息列表。</param>
    /// <returns></returns>
    public static List<ModuleInfo> Add(this List<ModuleInfo> modules, List<ModuleInfo> lists)
    {
        if (lists == null || lists.Count == 0)
            return modules;

        foreach (var module in lists)
        {
            if (module.Type == nameof(MenuType.Menu) || !modules.Exists(m => m.Url == module.Url))
                modules.Add(module);
        }
        modules = [.. modules.OrderBy(m => m.Sort)];
        return modules;
    }

    /// <summary>
    /// 添加一个模块信息。
    /// </summary>
    /// <param name="modules">模块列表。</param>
    /// <param name="id">ID。</param>
    /// <param name="name">名称。</param>
    /// <param name="icon">图标。</param>
    /// <param name="parentId">上级ID。</param>
    /// <param name="sort">排序。</param>
    /// <param name="url">URL。</param>
    /// <returns>菜单信息。</returns>
    public static ModuleInfo Add(this List<ModuleInfo> modules, string id, string name, string icon, string parentId, int sort, string url = null)
    {
        return AddItem(modules, parentId, id, name, icon, sort, url);
    }

    /// <summary>
    /// 添加一个模块信息。
    /// </summary>
    /// <param name="modules">模块列表。</param>
    /// <param name="parentId">上级ID。</param>
    /// <param name="id">ID。</param>
    /// <param name="name">名称。</param>
    /// <param name="icon">图标。</param>
    /// <param name="sort">排序。</param>
    /// <param name="url">URL。</param>
    /// <returns>菜单信息。</returns>
    public static ModuleInfo AddItem(this List<ModuleInfo> modules, string parentId, string id, string name, string icon, int sort, string url = null)
    {
        var info = new ModuleInfo
        {
            Id = id,
            Type = nameof(MenuType.Menu),
            Name = name,
            Icon = icon,
            ParentId = parentId,
            Sort = sort,
            Url = url,
            IsCode = true
        };
        if (!string.IsNullOrWhiteSpace(url))
        {
            info.Type = nameof(MenuType.Link);
            info.Target = nameof(LinkTarget.None);
        }
        modules.Add(info);
        return info;
    }

    /// <summary>
    /// 移除一个模块信息。
    /// </summary>
    /// <param name="modules">模块列表。</param>
    /// <param name="id">模块ID。</param>
    public static void Remove(this List<ModuleInfo> modules, string id)
    {
        if (modules == null || modules.Count == 0)
            return;

        var item = modules.FirstOrDefault(m => m.Id == id);
        if (item != null)
            modules.Remove(item);
    }

    /// <summary>
    /// 改变模块信息的上级ID。
    /// </summary>
    /// <param name="modules">模块列表。</param>
    /// <param name="id">模块ID。</param>
    /// <param name="parentId">上级模块ID。</param>
    public static void ChangeParent(this List<ModuleInfo> modules, string id, string parentId)
    {
        if (modules == null || modules.Count == 0)
            return;

        var item = modules.FirstOrDefault(m => m.Id == id);
        if (item != null)
            item.ParentId = parentId;
    }

    /// <summary>
    /// 将模块信息列表转成菜单信息列表。
    /// </summary>
    /// <param name="modules">模块信息列表。</param>
    /// <returns>菜单信息列表。</returns>
    public static List<MenuInfo> ToMenus(this List<ModuleInfo> modules)
    {
        if (modules == null || modules.Count == 0)
            return [];

        return modules.Where(m => m.Enabled).Select(m =>
        {
            var info = CreateMenu(m);
            if (!string.IsNullOrWhiteSpace(m.Url))
            {
                var route = Config.RouteTypes?.FirstOrDefault(r => r.Key == m.Url);
                route ??= Config.RouteTypes?.FirstOrDefault(r => r.Key?.StartsWith(m.Url) == true);
                if (route != null)
                    info.PageType = route.Value.Value;
            }
            return info;
        }).ToList();
    }

    /// <summary>
    /// 重新给模块列表排序。
    /// </summary>
    /// <param name="modules">模块列表。</param>
    public static void Resort(this List<ModuleInfo> modules)
    {
        if (modules == null || modules.Count == 0)
            return;

        var index = 1;
        foreach (var item in modules)
        {
            item.Sort = index++;
        }
    }

    internal static List<MenuInfo> ToMenuItems(this List<ModuleInfo> models, bool showRoot = true)
    {
        MenuInfo current = null;
        return models.ToMenuItems(ref current, showRoot);
    }

    internal static List<MenuInfo> ToMenuItems(this List<ModuleInfo> models, ref MenuInfo current, bool showRoot = true)
    {
        MenuInfo root = null;
        var menus = new List<MenuInfo>();
        if (showRoot)
        {
            root = root = Config.App.GetRootMenu();
            if (current != null && current.Id == root.Id)
                current = root;
            menus.Add(root);
        }
        if (models == null || models.Count == 0)
            return menus;

        var tops = models.Where(m => m.ParentId == "0").ToList();
        foreach (var item in tops)
        {
            //item.ParentName = Config.App.Name;
            //var menu = item.ToMenuInfo();
            var menu = CreateMenu(item, !showRoot);
            if (current != null && current.Id == menu.Id)
                current = menu;

            if (showRoot)
                root.Children.Add(menu);
            else
                menus.Add(menu);
            AddChildren(models, menu, ref current, !showRoot);
        }

        current ??= menus[0];
        return menus;
    }

    private static void AddChildren(List<ModuleInfo> models, MenuInfo menu, ref MenuInfo current, bool showUrl)
    {
        var items = models.Where(m => m.ParentId == menu.Id).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            //item.ParentName = menu.Name;
            //var sub = item.ToMenuInfo();
            var sub = CreateMenu(item, showUrl);
            sub.Parent = menu;
            if (current != null && current.Id == sub.Id)
                current = sub;

            menu.Children.Add(sub);
            AddChildren(models, sub, ref current, showUrl);
        }
    }

    private static MenuInfo CreateMenu(ModuleInfo info, bool showUrl = false)
    {
        return new MenuInfo
        {
            Data = info,
            Id = info.Id,
            Name = GetMenuName(info, showUrl),
            Icon = info.Icon,
            ParentId = info.ParentId,
            Type = info.Type,
            Target = info.Target,
            Url = info.Url,
            Sort = info.Sort,
            Enabled = info.Enabled,
            Layout = info.Layout,
            Plugins = info.Plugins
        };
    }

    private static string GetMenuName(ModuleInfo info, bool showUrl)
    {
        if (info.Target != Constants.Route || string.IsNullOrWhiteSpace(info.Url) || !showUrl)
            return info.Name;

        return $"{info.Name}({info.Url})";
    }
}