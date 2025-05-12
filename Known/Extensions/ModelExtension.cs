namespace Known.Extensions;

/// <summary>
/// 数据模型扩展类。
/// </summary>
public static class ModelExtension
{
    #region PageInfo
    internal static List<ActionInfo> GetToolItems(this PageInfo info)
    {
        if (info == null || info.Tools == null || info.Tools.Count == 0)
            return [];

        return [.. info.Tools.Select(t => new ActionInfo(t))];
    }

    internal static List<ActionInfo> GetActionItems(this PageInfo info)
    {
        if (info == null || info.Actions == null || info.Actions.Count == 0)
            return [];

        return [.. info.Actions.Select(a => new ActionInfo(a))];
    }

    internal static List<ColumnInfo> GetColumns<T>(this PageInfo info, FormInfo form)
    {
        if (info == null || info.Columns == null || info.Columns.Count == 0)
            return [];

        var properties = TypeHelper.Properties<T>();
        return info.Columns.OrderBy(t => t.Position).Select(c =>
        {
            var column = new ColumnInfo(c);
            SetColumn(column, form);
            var property = properties.FirstOrDefault(p => p.Name == column.Id);
            if (property != null)
                column.SetColumnInfo(property);
            return column;
        }).ToList();
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

    #region Organization
    internal static List<MenuInfo> ToMenuItems(this List<OrganizationInfo> models)
    {
        MenuInfo current = null;
        return models.ToMenuItems(ref current);
    }

    internal static List<MenuInfo> ToMenuItems(this List<OrganizationInfo> models, ref MenuInfo current)
    {
        var menus = new List<MenuInfo>();
        if (models == null || models.Count == 0)
            return menus;

        var tops = models.Where(m => m.ParentId == "0").OrderBy(m => m.Code).ToList();
        foreach (var item in tops)
        {
            var menu = CreateMenuInfo(item);
            if (current != null && current.Id == menu.Id)
                current = menu;

            menus.Add(menu);
            AddChildren(models, menu, ref current);
        }

        current ??= menus[0];
        return menus;
    }

    private static void AddChildren(List<OrganizationInfo> models, MenuInfo menu, ref MenuInfo current)
    {
        var items = models.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Code).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            item.ParentName = menu.Name;
            var sub = CreateMenuInfo(item);
            sub.Parent = menu;
            if (current != null && current.Id == sub.Id)
                current = sub;

            menu.Children.Add(sub);
            AddChildren(models, sub, ref current);
        }
    }

    private static MenuInfo CreateMenuInfo(OrganizationInfo model)
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