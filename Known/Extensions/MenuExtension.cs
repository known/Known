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
    /// <returns>菜单信息。</returns>
    public static MenuInfo AddItem(this List<MenuInfo> menus, string parentId, string id, string name, string icon, int sort, string url = null)
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
        menus.Add(info);
        return info;
    }

    /// <summary>
    /// 将菜单信息列表转成树形结构。
    /// </summary>
    /// <param name="menus">菜单信息列表。</param>
    /// <param name="showRoot">是否显示根节点。</param>
    /// <returns>树形菜单列表。</returns>
    public static List<MenuInfo> ToMenuItems(this List<MenuInfo> menus, bool showRoot = false)
    {
        MenuInfo root = null;
        var items = new List<MenuInfo>();
        if (showRoot)
        {
            root = Config.App.GetRootMenu();
            items.Add(root);
        }
        if (menus == null || menus.Count == 0)
            return items;

        var tops = menus.Where(m => m.ParentId == "0" && m.Target != Constants.Route).OrderBy(m => m.Sort).ToList();
        foreach (var item in tops)
        {
            if (item.Target == Constants.Route)
                continue;

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

    private static void AddChildren(List<MenuInfo> menus, MenuInfo menu)
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
            Enabled = info.Enabled,
            Layout = info.Layout,
            Plugins = info.Plugins,
            Color = info.Color,
            PageType = info.PageType
        };
    }

    internal static List<CodeInfo> GetAllActions(this MenuInfo info, Language language)
    {
        var codes = new List<CodeInfo>();
        var param = info.GetTablePageParameter();
        var page = param?.Page;
        if (page?.Tools != null && page?.Tools.Count > 0)
            codes.AddRange(page?.Tools.Select(b => GetAction(info, language, b)));
        if (page?.Actions != null && page?.Actions.Count > 0)
            codes.AddRange(page?.Actions.Select(b => GetAction(info, language, b)));
        return codes;
    }

    internal static List<CodeInfo> GetAllColumns(this MenuInfo info, Language language)
    {
        var codes = new List<CodeInfo>();
        var param = info.GetTablePageParameter();
        var page = param?.Page;
        if (page?.Columns != null && page?.Columns.Count > 0)
            codes.AddRange(page?.Columns.Select(c => GetColumn(info, language, c)));
        return codes;
    }

    private static CodeInfo GetAction(MenuInfo menu, Language language, string id)
    {
        var code = $"b_{menu.Id}_{id}";
        var button = Config.Actions.FirstOrDefault(b => b.Id == id);
        var name = button != null ? button.Name : id;
        name = language.GetText("Button", id, name);
        return new CodeInfo(code, name);
    }

    private static CodeInfo GetColumn(MenuInfo menu, Language language, PageColumnInfo info)
    {
        var code = $"c_{menu.Id}_{info.Id}";
        var name = language.GetText("", info.Id, info.Name);
        return new CodeInfo(code, name);
    }
}