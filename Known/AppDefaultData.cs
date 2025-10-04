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
        var methods = new List<(MethodInfo, ActionAttribute)>();
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        foreach (MethodInfo m in pageType.GetMethods(flags))
        {
            if (m.IsDefined(typeof(ActionAttribute), false))
            {
                var attr = m.GetCustomAttribute<ActionAttribute>(false);
                methods.Add((m, attr));
            }
        }
        foreach (var item in methods)
        {
            var method = item.Item1;
            var attr = item.Item2;
            var hasParameter = method.GetParameters().Length > 0;
            var action = Config.Actions.FirstOrDefault(b => b.Id == method.Name);
            if (action == null)
            {
                action = new ActionInfo
                {
                    Id = method.Name,
                    Name = attr.Name,
                    Icon = attr.Icon,
                    Style = attr.Style ?? "primary",
                    Position = hasParameter ? "Action" : "Toolbar"
                };
                Config.Actions.Add(action);
            }
            action.Title = attr.Title;
            action.Group = attr.Group;
            action.Visible = attr.Visible;
            action.Tabs = attr.Tabs;
            if (hasParameter)
                info.Page.Actions.Add(action);
            else
                info.Page.Tools.Add(action);
        }
    }

    private static void SetProperties(AutoPageInfo info, Type entityType)
    {
        var entity = new EntityInfo { Id = entityType.Name, Name = entityType.DisplayName() };
        var fields = TypeCache.Fields(entityType);
        foreach (var item in fields)
        {
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
}