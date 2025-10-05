namespace Known.Extensions;

/// <summary>
/// 菜单信息扩展类。
/// </summary>
public static class MenuExtension
{
    /// <summary>
    /// 获取系统菜单根节点。
    /// </summary>
    /// <param name="app">系统信息。</param>
    /// <returns></returns>
    public static MenuInfo GetRootMenu(this AppInfo app)
    {
        var root = new MenuInfo { Id = "0", Name = app.Name, Icon = "desktop" };
        root.Data = new SysModule { Id = root.Id, Name = root.Name };
        return root;
    }

    /// <summary>
    /// 添加一个菜单信息。
    /// </summary>
    /// <param name="menus">菜单列表。</param>
    /// <param name="parentId">上级ID。</param>
    /// <param name="id">ID。</param>
    /// <param name="name">名称。</param>
    /// <param name="icon">图标。</param>
    /// <param name="sort">排序。</param>
    /// <param name="url">URL。</param>
    /// <param name="target">打开目标。</param>
    /// <returns>菜单信息。</returns>
    public static MenuInfo AddItem(this List<MenuInfo> menus, string parentId, string id, string name, string icon, int sort, string url = null, string target = null)
    {
        var info = new MenuInfo
        {
            ParentId = parentId,
            Id = id,
            Type = nameof(MenuType.Menu),
            Name = name,
            Icon = icon,
            Sort = sort,
            Url = url
        };
        if (!string.IsNullOrWhiteSpace(url))
        {
            info.Type = nameof(MenuType.Link);
            info.Target = nameof(LinkTarget.None);
        }
        if (!string.IsNullOrWhiteSpace(target))
            info.Target = target;
        menus.Add(info);
        return info;
    }

    /// <summary>
    /// 修改页面菜单的上级ID。
    /// </summary>
    /// <typeparam name="T">页面类型。</typeparam>
    /// <param name="menus">菜单信息列表。</param>
    /// <param name="parentId">上级菜单ID。</param>
    /// <param name="sort">顺序。</param>
    public static void AddItem<T>(this List<MenuInfo> menus, string parentId, int sort)
    {
        if (menus == null)
            return;

        var item = menus.FirstOrDefault(m => m.PageType == typeof(T));
        if (item == null)
        {
            item = DataHelper.Routes.FirstOrDefault(m => m.PageType == typeof(T));
            if (item != null)
                menus.Add(item);
        }

        if (item != null)
        {
            item.ParentId = parentId;
            item.Sort = sort;
        }
    }

    /// <summary>
    /// 移除一个模块菜单。
    /// </summary>
    /// <param name="menus">模块菜单列表。</param>
    /// <param name="idOrName">模块ID或名称。</param>
    public static void Remove(this List<MenuInfo> menus, string idOrName)
    {
        var info = menus.FirstOrDefault(m => m.Id == idOrName || m.Name == idOrName);
        if (info != null)
        {
            menus.Remove(info);
        }
        else
        {
            //var item = DataHelper.Routes.FirstOrDefault(m => m.Id == idOrName || m.Name == idOrName);
            //if (item != null)
            //    DataHelper.Routes.Remove(item);
        }
    }

    /// <summary>
    /// 移除一个页面菜单。
    /// </summary>
    /// <typeparam name="T">页面类型。</typeparam>
    /// <param name="menus">菜单信息列表。</param>
    public static void Remove<T>(this List<MenuInfo> menus) where T : BaseComponent
    {
        if (menus == null || menus.Count == 0)
            return;

        var item = menus.FirstOrDefault(m => m.PageType == typeof(T));
        if (item != null)
            menus.Remove(item);
    }

    /// <summary>
    /// 修改页面菜单的上级ID。
    /// </summary>
    /// <typeparam name="T">页面类型。</typeparam>
    /// <param name="menus">菜单信息列表。</param>
    /// <param name="parentId">上级菜单ID。</param>
    /// <param name="sort">顺序。</param>
    public static void ChangeParent<T>(this List<MenuInfo> menus, string parentId, int sort)
    {
        if (menus == null || menus.Count == 0)
            return;

        var item = menus.FirstOrDefault(m => m.PageType == typeof(T));
        if (item != null)
        {
            menus.Where(m => m.ParentId == parentId && m.Sort >= sort).ToList().ForEach(m => ++m.Sort);
            item.ParentId = parentId;
            item.Sort = sort;
        }
    }

    /// <summary>
    /// 将模块信息列表转成树结构菜单信息列表。
    /// </summary>
    /// <param name="models">模块信息列表。</param>
    /// <param name="showRoot">是否显示根节点。</param>
    /// <returns></returns>
    public static List<MenuInfo> ToMenuItems(this List<MenuInfo> models, bool showRoot = true)
    {
        MenuInfo current = null;
        return models.ToMenuItems(ref current, showRoot);
    }

    internal static List<MenuInfo> ToMenuItems(this List<MenuInfo> models, ref MenuInfo current, bool showRoot = true)
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
            //var menu = CreateMenu(item, !showRoot);
            var menu = item.Clone();
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

    private static void AddChildren(List<MenuInfo> models, MenuInfo menu, ref MenuInfo current, bool showUrl)
    {
        var items = models.Where(m => m.ParentId == menu.Id).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            //item.ParentName = menu.Name;
            //var sub = item.ToMenuInfo();
            //var sub = CreateMenu(item, showUrl);
            var sub = item.Clone();
            sub.Parent = menu;
            if (current != null && current.Id == sub.Id)
                current = sub;

            menu.Children.Add(sub);
            AddChildren(models, sub, ref current, showUrl);
        }
    }

    //private static MenuInfo CreateMenu(MenuInfo info, bool showUrl = false)
    //{
    //    return new MenuInfo
    //    {
    //        Data = info,
    //        Id = info.Id,
    //        Name = GetMenuName(info, showUrl),
    //        Icon = info.Icon,
    //        ParentId = info.ParentId,
    //        Type = info.Type,
    //        Target = info.Target,
    //        Url = info.Url,
    //        Sort = info.Sort,
    //        Enabled = info.Enabled,
    //        IsCode = info.IsCode,
    //        Layout = info.Layout,
    //        Plugins = info.Plugins
    //    };
    //}

    //private static string GetMenuName(MenuInfo info, bool showUrl)
    //{
    //    if (info.Target != Constants.Route || string.IsNullOrWhiteSpace(info.Url) || !showUrl)
    //        return info.Name;

    //    return $"{info.Name}({info.Url})";
    //}

    /// <summary>
    /// 将菜单信息列表转成树形结构。
    /// </summary>
    /// <param name="menus">菜单信息列表。</param>
    /// <param name="showRoot">是否显示根节点。</param>
    /// <returns>树形菜单列表。</returns>
    public static List<MenuInfo> ToMenuItems(this IEnumerable<MenuInfo> menus, bool showRoot = false)
    {
        MenuInfo root = null;
        var items = new List<MenuInfo>();
        if (showRoot)
        {
            root = Config.App.GetRootMenu();
            items.Add(root);
        }
        if (menus == null || !menus.Any())
            return items;

        var tops = menus.Where(m => m.ParentId == "0").OrderBy(m => m.Sort).ToList();
        foreach (var item in tops)
        {
            var menu = CreateMenu(item);
            if (showRoot)
                root.AddChild(menu);
            else
                items.Add(menu);
            AddChildren(menus, menu);
        }
        return items;
    }

    internal static void Resort(this List<MenuInfo> menus)
    {
        if (menus == null || menus.Count == 0)
            return;

        var index = 1;
        foreach (var item in menus)
        {
            item.Sort = index++;
        }
    }

    private static void AddChildren(IEnumerable<MenuInfo> menus, MenuInfo menu)
    {
        var items = menus.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Sort).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            var sub = CreateMenu(item);
            menu.AddChild(sub);
            AddChildren(menus, sub);
        }
    }

    private static MenuInfo CreateMenu(MenuInfo info)
    {
        return new MenuInfo
        {
            Id = info.Id,
            ParentId = info.ParentId,
            Code = info.Code,
            Name = info.Name,
            Icon = info.Icon,
            Description = info.Description,
            Type = info.Type,
            Target = info.Target,
            Url = info.Url,
            Sort = info.Sort,
            Visible = info.Visible,
            Enabled = info.Enabled,
            CanEdit = info.CanEdit,
            IsCode = info.IsCode,
            Badge = info.Badge,
            Layout = info.Layout,
            Plugins = info.Plugins,
            Color = info.Color,
            PageType = info.PageType,
            Data = info.Data
        };
    }
}