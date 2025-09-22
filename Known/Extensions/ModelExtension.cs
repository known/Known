namespace Known.Extensions;

/// <summary>
/// 数据模型扩展类。
/// </summary>
public static class ModelExtension
{
    #region ActionInfo
    /// <summary>
    /// 添加一个操作按钮。
    /// </summary>
    /// <param name="items">操作列表。</param>
    /// <param name="idOrName">按钮ID或名称。</param>
    /// <param name="title">按钮提示信息。</param>
    public static void Add(this List<ActionInfo> items, string idOrName, string title = "")
    {
        items.Add(new ActionInfo(idOrName) { Title = title });
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
        items.Add(new ActionInfo(idOrName) { Group = group, Title = title });
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
        items.Add(new ActionInfo(idOrName) { Badge = badge, Title = title });
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
        items.Add(new ActionInfo
        {
            Id = id,
            Name = name,
            Icon = icon,
            Title = title
        });
    }

    internal static void TabChange(this List<ActionInfo> actions, string tab)
    {
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
    /// <summary>
    /// 转换成实体信息对象。
    /// </summary>
    /// <returns></returns>
    public static EntityInfo ToEntity(this AutoPageInfo page)
    {
        if (!string.IsNullOrWhiteSpace(page.EntityData))
            return DataHelper.ToEntity(page.EntityData);

        var info = new EntityInfo { Id = page.Id, Name = page.Name, TableName = page.Script };
        foreach (var item in page.Form.Fields)
        {
            var field = item.ToField();
            field.IsForm = true;
            field.IsGrid = page.Page.Columns.Exists(d => d.Id == field.Id);
            info.Fields.Add(field);
        }
        return info;
    }

    //internal static List<ActionInfo> GetToolItems(this PageInfo info, Type pageType)
    //{
    //    if (info == null || info.Tools == null || info.Tools.Count == 0)
    //        return [];

    //    var items = info.Tools.Select(t => new ActionInfo(t)).ToList();
    //    //if (!info.UseCodeConfig)
    //    //    return items;

    //    //foreach (var item in items)
    //    //{
    //    //    SetAction(item, pageType);
    //    //}
    //    return items;
    //}

    //internal static List<ActionInfo> GetActionItems(this PageInfo info, Type pageType)
    //{
    //    if (info == null || info.Actions == null || info.Actions.Count == 0)
    //        return [];

    //    var items = info.Actions.Select(a => new ActionInfo(a)).ToList();
    //    //if (!info.UseCodeConfig)
    //    //    return items;

    //    //foreach (var item in items)
    //    //{
    //    //    SetAction(item, pageType);
    //    //}
    //    return items;
    //}

    //private static void SetAction(ActionInfo item, Type pageType)
    //{
    //    try
    //    {
    //        var method = pageType.GetMethod(item.Id);
    //        var attr = method?.GetCustomAttribute<ActionAttribute>();
    //        if (attr != null)
    //        {
    //            if (!string.IsNullOrWhiteSpace(attr.Icon))
    //                item.Icon = attr.Icon;
    //            if (!string.IsNullOrWhiteSpace(attr.Name))
    //                item.Name = attr.Name;
    //            item.Title = attr.Title;
    //            item.Style = attr.Style;
    //            item.Visible = attr.Visible;
    //            item.Group = attr.Group;
    //            item.Tabs = attr.Tabs;
    //        }
    //    }
    //    catch { }
    //}

    internal static List<ColumnInfo> GetColumns<T>(this PageInfo info, FormInfo form)
    {
        if (info == null || info.Columns == null || info.Columns.Count == 0)
            return [];

        //if (!info.UseCodeConfig)
        //{
            return info.Columns.OrderBy(t => t.Position).Select(c =>
            {
                var column = new ColumnInfo(c);
                SetColumn(column, form);
                return column;
            }).ToList();
        //}

        //var properties = TypeHelper.Properties<T>();
        //return info.Columns.OrderBy(t => t.Position).Select(c =>
        //{
        //    var column = new ColumnInfo(c);
        //    SetColumn(column, form);
        //    var property = properties.FirstOrDefault(p => p.Name == column.Id);
        //    if (property != null)
        //        column.SetColumnInfo(property);
        //    return column;
        //}).ToList();
    }

    private static void SetColumn(ColumnInfo column, FormInfo form)
    {
        var item = form?.Fields?.FirstOrDefault(f => f.Id == column.Id);
        if (item != null)
        {
            //column.Type = item.Type;
            column.Category = item.Category;
            column.Unit = item.Unit;
        }
        //if (column.Type == FieldType.Text)
        //{
        //    EntityInfo model = form?.Model;
        //    var field = model?.Fields?.FirstOrDefault(f => f.Id == column.Id);
        //    if (field != null)
        //        column.Type = field.Type;
        //}
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