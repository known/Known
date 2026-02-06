namespace Known.Extensions;

/// <summary>
/// 数据模型扩展类。
/// </summary>
public static class ModelExtension
{
    #region Menu
    internal static List<CodeInfo> GetAllActions(this MenuInfo info, Language language)
    {
        var codes = new List<CodeInfo>();
        var param = info.GetAutoPageParameter();
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
        var param = info.GetAutoPageParameter();
        var page = param?.Page;
        if (page?.Columns != null && page?.Columns.Count > 0)
            codes.AddRange(page?.Columns.Select(c => GetColumn(info, language, c)));
        return codes;
    }

    private static CodeInfo GetAction(MenuInfo menu, Language language, ActionInfo info)
    {
        var code = $"b_{menu.Id}_{info.Id}";
        return new CodeInfo(code, language[info.Name]);
    }

    private static CodeInfo GetColumn(MenuInfo menu, Language language, PageColumnInfo info)
    {
        var code = $"c_{menu.Id}_{info.Id}";
        return new CodeInfo(code, language[info.Name]);
    }
    #endregion

    #region ActionInfo
    /// <summary>
    /// 添加一个操作按钮。
    /// </summary>
    /// <param name="items">操作列表。</param>
    /// <param name="idOrName">按钮ID或名称。</param>
    /// <param name="title">按钮提示信息。</param>
    public static void Add(this List<ActionInfo> items, string idOrName, string title = "")
    {
        items?.Add(new ActionInfo(idOrName) { Title = title });
    }

    /// <summary>
    /// 添加一个操作按钮。
    /// </summary>
    /// <param name="items">操作列表。</param>
    /// <param name="idOrName">按钮ID或名称。</param>
    /// <param name="group">按钮分组。</param>
    /// <param name="title">按钮提示信息。</param>
    public static void Add(this List<ActionInfo> items, string idOrName, string group, string title = "")
    {
        items?.Add(new ActionInfo(idOrName) { Group = group, Title = title });
    }

    /// <summary>
    /// 添加一个操作按钮。
    /// </summary>
    /// <param name="items">操作列表。</param>
    /// <param name="idOrName">按钮ID或名称。</param>
    /// <param name="badge">徽章数量。</param>
    /// <param name="title">按钮提示信息。</param>
    public static void Add(this List<ActionInfo> items, string idOrName, int badge, string title = "")
    {
        items?.Add(new ActionInfo(idOrName) { Badge = badge, Title = title });
    }

    /// <summary>
    /// 添加一个操作按钮。
    /// </summary>
    /// <param name="items">操作列表。</param>
    /// <param name="id">按钮ID。</param>
    /// <param name="name">按钮名称。</param>
    /// <param name="icon">按钮图标。</param>
    /// <param name="title">按钮提示信息。</param>
    public static void Add(this List<ActionInfo> items, string id, string name, string icon, string title = "")
    {
        items?.Add(new ActionInfo { Id = id, Name = name, Icon = icon, Title = title });
    }

    internal static List<ActionInfo> Format(this List<ActionInfo> actions)
    {
        if (actions == null || actions.Count == 0)
            return [];

        foreach (var item in actions)
        {
            var info = Config.Actions.FirstOrDefault(d => d.Id == item.Id);
            if (info != null)
            {
                item.Name = info.Name;
                item.Icon = info.Icon;
                item.Style = info.Style;
                if (!string.IsNullOrWhiteSpace(info.Group))
                    item.Group = info.Group;
            }
        }
        return actions;
    }

    internal static void TabChange(this List<ActionInfo> actions, string tab)
    {
        if (actions == null || actions.Count == 0)
            return;

        foreach (var item in actions)
        {
            if (item.Tabs == null || item.Tabs.Length == 0)
                continue;

            item.Visible = item.Tabs.Contains(tab);
        }
    }

    internal static List<ActionInfo> GetGroupItems(this List<ActionInfo> actions)
    {
        var infos = new List<ActionInfo>();
        var items = actions.Where(i => i.Visible).ToList();
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.Group))
            {
                infos.Add(item);
                continue;
            }

            var group = infos.FirstOrDefault(d => d.Id == item.Group);
            if (group == null)
            {
                group = new ActionInfo(item.Group);
                infos.Add(group);
            }
            group.Children.Add(item);
        }
        return infos;
    }
    #endregion

    #region PageInfo
    internal static List<ColumnInfo> GetColumns<T>(this PageInfo info)
    {
        if (info == null || info.Columns == null || info.Columns.Count == 0)
            return [];

        if (typeof(T).IsDictionary())
            return [.. info.Columns.OrderBy(t => t.Position).Select(c => new ColumnInfo(c))];

        var properties = TypeHelper.Properties<T>().ToList();
        var columns = info.Columns.Where(d => properties.Exists(p => p.Name == d.Id));
        return [.. columns.OrderBy(t => t.Position).Select(c => new ColumnInfo(c))];
    }
    #endregion

    #region ComponentInfo
    /// <summary>
    /// 设置组件信息。
    /// </summary>
    /// <typeparam name="T">组件类型。</typeparam>
    /// <param name="components">组件字典。</param>
    /// <param name="id">组件ID。</param>
    /// <param name="title">Tab标题。</param>
    /// <param name="parameters">组件参数。</param>
    public static void Set<T>(this Dictionary<string, ComponentInfo> components, int id, string title, Dictionary<string, object> parameters = null)
    {
        components[title] = new ComponentInfo { Id = id, Type = typeof(T), Parameters = parameters };
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
            var menu = item.ToMenuInfo();
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
            var sub = item.ToMenuInfo();
            sub.Parent = menu;
            if (current != null && current.Id == sub.Id)
                current = sub;

            menu.Children.Add(sub);
            AddChildren(models, sub, ref current);
        }
    }

    internal static MenuInfo ToMenuInfo(this SysOrganization model)
    {
        return new MenuInfo
        {
            Id = model.Id,
            ParentId = model.ParentId,
            Code = model.Code,
            Name = model.Name,
            Data = model
        };
    }
    #endregion

    #region Flow
    /// <summary>
    /// 获取工作流步骤项目列表。
    /// </summary>
    /// <param name="info">工作流配置信息。</param>
    /// <returns>步骤项目列表。</returns>
    public static List<ItemModel> GetFlowStepItems(this FlowInfo info)
    {
        if (info == null || info.Steps == null || info.Steps.Count == 0)
            return null;

        return info.Steps.Select(s => new ItemModel(s.Id, s.Name)).ToList();
    }
    #endregion
}