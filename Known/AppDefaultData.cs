namespace Known;

class AppDefaultData
{
    internal static void Load(AppDataInfo data)
    {
        // 加载顶部导航
        if (data.TopNavs.Count == 0)
            data.TopNavs = LoadTopNavs();
        // 加载配置的一级模块
        LoadModules(data);
        // 加载Menu特性的组件菜单
        LoadMenus(data);
    }

    private static List<PluginInfo> LoadTopNavs()
    {
        return Config.Plugins.Where(p => p.IsNavComponent)
                             .OrderBy(p => p.Attribute.Sort)
                             .Select(p => new PluginInfo { Id = p.Id, Type = p.Id })
                             .ToList();
    }

    private static void LoadModules(AppDataInfo data)
    {
        foreach (var item in Config.Modules)
        {
            if (!data.Modules.Exists(m => m.Id == item.Id))
                data.Modules.Add(item);
        }
    }

    private static void LoadMenus(AppDataInfo data)
    {
        foreach (var item in Config.Menus)
        {
            if (data.Modules.Exists(m => m.Id == item.Page.Name))
                continue;

            if (!data.Modules.Exists(m => m.Id == item.Parent))
                continue;

            var info = new ModuleInfo
            {
                Id = item.Page.Name,
                Type = nameof(MenuType.Link),
                Name = item.Name,
                Icon = item.Icon,
                ParentId = item.Parent,
                Sort = item.Sort,
                Url = item.Url,
                Target = nameof(LinkTarget.None)
            };
            if (TypeHelper.IsSubclassOfGeneric(item.Page, typeof(BaseTablePage<>), out var types))
                info.Plugins.AddPlugin(CreateTablePage(item.Page, types[0]));
            data.Modules.Add(info);
        }
    }

    private static TablePageInfo CreateTablePage(Type pageType, Type entityType)
    {
        var info = new TablePageInfo
        {
            Page = new PageInfo { Type = pageType.FullName, ShowPager = true, PageSize = Config.App.DefaultPageSize },
            Form = new FormInfo()
        };
        SetMethods(info, pageType);
        SetProperties(info, entityType);
        return info;
    }

    private static void SetMethods(TablePageInfo info, Type pageType)
    {
        var methods = pageType.GetMethods();
        foreach (var item in methods)
        {
            if (item.GetCustomAttribute<ActionAttribute>() is not null)
            {
                if (item.GetParameters().Length == 0)
                    info.Page.Tools.Add(item.Name);
                else
                    info.Page.Actions.Add(item.Name);
            }
        }
    }

    private static void SetProperties(TablePageInfo info, Type entityType)
    {
        var properties = TypeHelper.Properties(entityType);
        foreach (var item in properties)
        {
            var column = item.GetCustomAttribute<ColumnAttribute>();
            if (column != null)
                SetPageColumns(info, item, column);

            var form = item.GetCustomAttribute<FormAttribute>();
            if (form != null)
                SetFormFields(info, item, form);
        }
    }

    private static void SetPageColumns(TablePageInfo info, PropertyInfo item, ColumnAttribute column)
    {
        info.Page.Columns.Add(new PageColumnInfo
        {
            Id = item.Name,
            Name = item.DisplayName(),
            Category = item.Category(),
            Width = item.GetColumnWidth(),
            IsSum = column.IsSum,
            IsSort = column.IsSort,
            DefaultSort = column.DefaultSort,
            IsViewLink = column.IsViewLink,
            IsQuery = column.IsQuery,
            IsQueryAll = column.IsQueryAll,
            Type = column.Type,
            Fixed = column.Fixed,
            Align = column.Align
        });
    }

    private static void SetFormFields(TablePageInfo info, PropertyInfo item, FormAttribute form)
    {
        info.Form.Fields.Add(new FormFieldInfo
        {
            Id = item.Name,
            Name = item.DisplayName(),
            Category = item.Category(),
            Required = item.IsRequired(),
            Row = form.Row,
            Column = form.Column,
            Type = Utils.ConvertTo<FieldType>(form.Type),
            CustomField = form.CustomField,
            ReadOnly = form.ReadOnly,
            Placeholder = form.Placeholder
        });
    }
}