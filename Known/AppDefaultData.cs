namespace Known;

class AppDefaultData
{
    internal static AutoPageInfo CreateAutoPage(Type pageType, Type entityType = null)
    {
        var info = new AutoPageInfo();
        info.Page.Type = pageType.FullName;
        SetMethods(info, pageType);
        if (entityType != null)
        {
            info.Page.ShowPager = true;
            info.Page.PageSize = Config.App.DefaultPageSize;
            SetProperties(info, entityType);
        }
        return info;
    }

    private static void SetMethods(AutoPageInfo info, Type pageType)
    {
        var items = pageType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .OrderByDescending(m => GetInheritanceDepth(pageType, m.DeclaringType));
        foreach (var item in items)
        {
            if (item.IsDefined(typeof(ActionAttribute), false))
            {
                if (!UIConfig.IsAdvAdmin && pageType.Assembly == Config.Frame && Config.AdvMethods.Contains(item.Name))
                    continue;

                var attr = item.GetCustomAttribute<ActionAttribute>(false);
                var hasParameter = item.GetParameters().Length > 0;
                var config = Config.Actions.FirstOrDefault(b => b.Id == item.Name);
                var action = new ActionInfo
                {
                    Id = item.Name,
                    Name = attr.Name ?? config?.Name ?? item.Name,
                    Icon = attr.Icon ?? config?.Icon,
                    Style = attr.Style ?? config?.Style ?? "primary",
                    Position = hasParameter ? "Action" : "Toolbar",
                    Title = attr.Title ?? config?.Title,
                    Group = attr.Group ?? config?.Group,
                    Visible = attr.Visible,
                    Tabs = attr.Tabs
                };
                Language.DefaultDatas.Add(action.Name);
                if (config == null)
                    Config.Actions.Add(action);
                if (hasParameter)
                    info.Page.Actions.Add(action);
                else
                    info.Page.Tools.Add(action);
            }
        }
    }

    private static void SetProperties(AutoPageInfo info, Type entityType)
    {
        var entity = new EntityInfo { Id = entityType.Name, Name = entityType.DisplayName() };
        var fields = TypeCache.Fields(entityType);
        foreach (var item in fields)
        {
            Language.DefaultDatas.Add(item.DisplayName);

            var column = item.GetPageColumn();
            if (column != null)
                info.Page.Columns.Add(column);

            var form = item.GetFormField();
            if (form != null)
                info.Form.Fields.Add(form);

            var field = item.GetField();
            if (field != null && !string.IsNullOrWhiteSpace(field.Name))
                entity.Fields.Add(field);
        }
        info.EntityData = DataHelper.ToEntityData(entity);
    }

    private static int GetInheritanceDepth(Type currentType, Type declaringType)
    {
        int depth = 0;
        Type baseType = currentType;
        while (baseType != null && baseType != declaringType)
        {
            baseType = baseType.BaseType;
            depth++;
        }
        // 如果 declaringType 不在当前类型的继承链中（罕见情况，例如接口方法），返回 int.MaxValue 排在最后
        return baseType == null ? int.MaxValue : depth;
    }
}