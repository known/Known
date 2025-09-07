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

    internal static void Load(AppDataInfo data)
    {
        if (data.TopNavs.Count == 0)
            data.TopNavs = LoadTopNavs();
        LoadModules(data);
        LoadMenus(data);
    }

    internal static void LoadModule(AppDataInfo data)
    {
        // 添加默认一级模块
        data.Modules.AddItem("0", Constants.BaseData, "基础数据", "database", 1);
        data.Modules.AddItem<CompanyForm>(Constants.BaseData, 1);
        data.Modules.AddItem<SysDictionaryList>(Constants.BaseData, 2);
        data.Modules.AddItem<SysOrganizationList>(Constants.BaseData, 3);
        data.Modules.AddItem("0", Constants.System, "系统管理", "setting", 99);
        data.Modules.AddItem<SysSystem>(Constants.System, 1);
        data.Modules.AddItem<SysRoleList>(Constants.System, 2);
        data.Modules.AddItem<SysUserList>(Constants.System, 3);
        data.Modules.AddItem<SysTaskList>(Constants.System, 4);
        data.Modules.AddItem<SysFileList>(Constants.System, 5);
        data.Modules.AddItem<SysLogList>(Constants.System, 6);
    }

    // 加载顶部导航
    private static List<PluginInfo> LoadTopNavs()
    {
        var infos = new List<PluginInfo>();
        foreach (var item in PluginConfig.TopNavs)
        {
            if (item.Type == typeof(NavFontSize) && !Config.App.IsSize)
                continue;
            if (item.Type == typeof(NavLanguage) && !Config.App.IsLanguage)
                continue;
            if (item.Type == typeof(NavTheme) && !Config.App.IsTheme)
                continue;
            infos.Add(new PluginInfo { Id = item.Id, Type = item.Id });
        }
        return infos;
    }

    // 加载配置的一级模块
    private static void LoadModules(AppDataInfo data)
    {
        foreach (var item in Config.Modules)
        {
            if (!data.Modules.Exists(m => m.Id == item.Id))
                data.Modules.Add(item);
        }
    }

    // 加载Menu特性的组件菜单
    private static void LoadMenus(AppDataInfo data)
    {
        foreach (var item in Config.Menus)
        {
            //if (item.Parent != "0" && !data.Modules.Exists(m => m.Id == item.Parent))
            //    continue;

            AddModule(data, item);
        }
    }

    private static void AddModule(AppDataInfo data, MenuAttribute item)
    {
        var info = data.Modules.FirstOrDefault(m => m.Id == item.Page.FullName);
        if (info == null)
        {
            info = new ModuleInfo
            {
                Id = item.Page.FullName,
                Type = nameof(MenuType.Link),
                Name = item.Name,
                Icon = item.Icon,
                ParentId = item.Parent,
                Sort = item.Sort,
                Url = item.Url,
                Target = nameof(LinkTarget.None),
                IsCode = true
            };
            data.Modules.Add(info);
        }
        var table = AppData.CreateAutoPage(item.Page);
        if (table != null)
            info.Plugins.AddPlugin(table);
    }

    private static void SetMethods(AutoPageInfo info, Type pageType)
    {
        var actions = AppData.GetActions();
        var methods = pageType.GetMethods();
        foreach (var item in methods)
        {
            var attr = item.GetCustomAttribute<ActionAttribute>();
            if (attr != null)
            {
                var hasParameter = item.GetParameters().Length > 0;
                var action = actions?.FirstOrDefault(b => b.Id == item.Name);
                if (action == null)
                {
                    // 将代码定义的按钮添加到按钮列表中
                    action = new ActionInfo
                    {
                        Id = item.Name,
                        Name = attr.Name,
                        Icon = attr.Icon,
                        Title = attr.Title,
                        Tabs = attr.Tabs,
                        Style = "primary",
                        Position = hasParameter ? "Action" : "Toolbar"
                    };
                    Config.Actions.Add(action);
                }

                if (!hasParameter)
                    info.Page.Tools.Add(item.Name);
                else
                    info.Page.Actions.Add(item.Name);
            }
        }
    }

    private static void SetProperties(AutoPageInfo info, Type entityType)
    {
        var entity = new EntityInfo { Id = entityType.Name, Name = entityType.DisplayName() };
        var properties = TypeHelper.Properties(entityType);
        foreach (var item in properties)
        {
            var column = item.GetCustomAttribute<ColumnAttribute>();
            if (column != null)
                SetPageColumns(info, item, column);

            var form = item.GetCustomAttribute<FormAttribute>();
            if (form != null)
                SetFormFields(info, item, form);

            var field = GetField(item);
            if (field != null)
                entity.Fields.Add(field);
        }
        info.EntityData = DataHelper.ToEntityData(entity);
    }

    private static void SetPageColumns(AutoPageInfo info, PropertyInfo item, ColumnAttribute column)
    {
        var type = column.Type;
        if (type == FieldType.Text)
            type = item.GetFieldType();
        info.Page.Columns.Add(new PageColumnInfo
        {
            Id = item.Name,
            Name = item.DisplayName(),
            Length = item.GetFieldLength(),
            Required = item.IsRequired(),
            Category = item.Category(),
            Width = column.Width > 0 ? column.Width : item.GetColumnWidth(type),
            Ellipsis = column.Ellipsis,
            IsSum = column.IsSum,
            IsSort = column.IsSort,
            DefaultSort = column.DefaultSort,
            IsViewLink = column.IsViewLink,
            IsQuery = column.IsQuery,
            IsQueryAll = column.IsQueryAll,
            QueryValue = column.QueryValue,
            Type = type,
            Fixed = column.Fixed,
            Align = column.Align
        });
    }

    private static void SetFormFields(AutoPageInfo info, PropertyInfo item, FormAttribute form)
    {
        var type = FieldType.Text;
        if (!string.IsNullOrWhiteSpace(form.Type))
            type = Utils.ConvertTo<FieldType>(form.Type);
        if (type == FieldType.Text)
            type = item.PropertyType.GetFieldType();
        info.Form.Fields.Add(new FormFieldInfo
        {
            Id = item.Name,
            Name = item.DisplayName(),
            Category = item.Category(),
            Length = item.GetFieldLength(),
            Required = item.IsRequired(),
            Row = form.Row,
            Column = form.Column,
            Type = type,
            CustomField = form.CustomField,
            ReadOnly = form.ReadOnly,
            Placeholder = form.Placeholder,
            FieldValue = form.FieldValue,
            Rows = form.Rows,
            Unit = form.Unit
        });
    }

    private static FieldInfo GetField(PropertyInfo item)
    {
        var name = item.DisplayName();
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return new FieldInfo
        {
            Id = item.Name,
            Name = name,
            Length = item.GetFieldLength(),
            Required = item.IsRequired(),
            Type = item.GetFieldType()
        };
    }
}