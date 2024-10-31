namespace Known.Extensions;

/// <summary>
/// 数据模型扩展类。
/// </summary>
public static class ModelExtension
{
    #region CodeInfo
    /// <summary>
    /// 往代码表列表中插入空文本字符串。
    /// </summary>
    /// <param name="codes">代码表列表。</param>
    /// <param name="emptyText">空文本字符串，默认空。</param>
    /// <returns>新代码表列表。</returns>
    public static List<CodeInfo> ToCodes(this List<CodeInfo> codes, string emptyText = "")
    {
        var infos = new List<CodeInfo>();
        if (!string.IsNullOrWhiteSpace(emptyText))
            infos.Add(new CodeInfo("", emptyText));

        if (codes != null && codes.Count > 0)
            infos.AddRange(codes);

        return infos;
    }
    #endregion

    #region Menu
    /// <summary>
    /// 将菜单信息列表转成树形结构。
    /// </summary>
    /// <param name="menus">菜单信息列表。</param>
    /// <returns>树形菜单列表。</returns>
    public static List<MenuInfo> ToMenuItems(this List<MenuInfo> menus)
    {
        var items = new List<MenuInfo>();
        if (menus == null || menus.Count == 0)
            return items;

        var tops = menus.Where(m => m.ParentId == "0" && m.Target != Constants.Route).OrderBy(m => m.Sort).ToList();
        foreach (var item in tops)
        {
            if (item.Target == Constants.Route)
                continue;

            var menu = new MenuInfo(item);
            items.Add(menu);
            AddChildren(menus, menu);
        }
        return items;
    }

    private static void AddChildren(List<MenuInfo> menus, MenuInfo menu)
    {
        var items = menus.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Sort).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            var sub = new MenuInfo(item);
            sub.Parent = menu;
            menu.Children.Add(sub);
            AddChildren(menus, sub);
        }
    }
    #endregion

    #region Module
    internal static List<MenuInfo> ToMenuItems(this List<SysModule> models, bool showRoot = true)
    {
        MenuInfo current = null;
        return models.ToMenuItems(ref current, showRoot);
    }

    internal static List<MenuInfo> ToMenuItems(this List<SysModule> models, ref MenuInfo current, bool showRoot = true)
    {
        MenuInfo root = null;
        var menus = new List<MenuInfo>();
        if (showRoot)
        {
            root = new MenuInfo("0", Config.App.Name, "desktop");
            if (current != null && current.Id == root.Id)
                current = root;

            root.Data = new SysModule { Id = root.Id, Name = root.Name };
            menus.Add(root);
        }
        if (models == null || models.Count == 0)
            return menus;

        var tops = models.Where(m => m.ParentId == "0").OrderBy(m => m.Sort).ToList();
        foreach (var item in tops)
        {
            item.ParentName = Config.App.Name;
            var menu = new MenuInfo(item);
            if (current != null && current.Id == menu.Id)
                current = menu;

            if (showRoot)
                root.Children.Add(menu);
            else
                menus.Add(menu);
            AddChildren(models, menu, ref current);
        }

        current ??= menus[0];
        return menus;
    }

    private static void AddChildren(List<SysModule> models, MenuInfo menu, ref MenuInfo current)
    {
        var items = models.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Sort).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            item.ParentName = menu.Name;
            var sub = new MenuInfo(item);
            sub.Parent = menu;
            if (current != null && current.Id == sub.Id)
                current = sub;

            menu.Children.Add(sub);
            AddChildren(models, sub, ref current);
        }
    }
    #endregion

    #region Organization
    internal static List<MenuInfo> ToMenuItems(this List<SysOrganization> models)
    {
        MenuInfo current = null;
        return models.ToMenuItems(ref current);
    }

    internal static List<MenuInfo> ToMenuItems(this List<SysOrganization> models, ref MenuInfo current)
    {
        var menus = new List<MenuInfo>();
        if (models == null || models.Count == 0)
            return menus;

        var tops = models.Where(m => m.ParentId == "0").OrderBy(m => m.Code).ToList();
        foreach (var item in tops)
        {
            var menu = new MenuInfo(item);
            if (current != null && current.Id == menu.Id)
                current = menu;

            menus.Add(menu);
            AddChildren(models, menu, ref current);
        }

        current ??= menus[0];
        return menus;
    }

    private static void AddChildren(List<SysOrganization> models, MenuInfo menu, ref MenuInfo current)
    {
        var items = models.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Code).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            item.ParentName = menu.Name;
            var sub = new MenuInfo(item);
            sub.Parent = menu;
            if (current != null && current.Id == sub.Id)
                current = sub;

            menu.Children.Add(sub);
            AddChildren(models, sub, ref current);
        }
    }
    #endregion
}