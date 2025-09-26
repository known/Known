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
        root.Data = new ModuleInfo { Id = root.Id, Name = root.Name };
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
            var item = DataHelper.Routes.FirstOrDefault(m => m.Id == idOrName || m.Name == idOrName);
            if (item != null)
                DataHelper.Routes.Remove(item);
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