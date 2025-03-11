﻿namespace Known;

class AppDefaultData
{
    internal static void Load(AppDataInfo data)
    {
        if (data.TopNavs.Count == 0)
            data.TopNavs = LoadTopNavs();
        LoadModules(data);
        LoadMenus(data);
    }

    // 加载顶部导航
    private static List<PluginInfo> LoadTopNavs()
    {
        var plugins = Config.Plugins.Where(p => p.IsNavComponent).OrderBy(p => p.Sort).ToList();
        var infos = new List<PluginInfo>();
        foreach (var item in plugins)
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
            if (!data.Modules.Exists(m => m.Id == item.Parent))
                continue;

            var info = data.Modules.FirstOrDefault(m => m.Id == item.Page.Name);
            if (info == null)
            {
                info = new ModuleInfo
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
                data.Modules.Add(info);
            }
            var table = AppData.CreateAutoPage(item.Page);
            if (table != null)
                info.Plugins.AddPlugin(table);
        }
    }

    internal static AutoPageInfo CreateAutoPage(Type pageType, Type entityType)
    {
        var info = new AutoPageInfo
        {
            Page = new PageInfo { Type = pageType.FullName, ShowPager = true, PageSize = Config.App.DefaultPageSize },
            Form = new FormInfo()
        };
        SetMethods(info, pageType);
        SetProperties(info, entityType);
        return info;
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

    private static void SetPageColumns(AutoPageInfo info, PropertyInfo item, ColumnAttribute column)
    {
        info.Page.Columns.Add(new PageColumnInfo
        {
            Id = item.Name,
            Name = item.DisplayName(),
            Length = item.GetFieldLength(),
            Required = item.IsRequired(),
            Category = item.Category(),
            Width = item.GetColumnWidth(),
            IsSum = column.IsSum,
            IsSort = column.IsSort,
            DefaultSort = column.DefaultSort,
            IsViewLink = column.IsViewLink,
            IsQuery = column.IsQuery,
            IsQueryAll = column.IsQueryAll,
            Type = item.GetFieldType(),
            Fixed = column.Fixed,
            Align = column.Align
        });
    }

    private static void SetFormFields(AutoPageInfo info, PropertyInfo item, FormAttribute form)
    {
        info.Form.Fields.Add(new FormFieldInfo
        {
            Id = item.Name,
            Name = item.DisplayName(),
            Category = item.Category(),
            Length = item.GetFieldLength(),
            Required = item.IsRequired(),
            Row = form.Row,
            Column = form.Column,
            Type = item.GetFieldType(),
            CustomField = form.CustomField,
            ReadOnly = form.ReadOnly,
            Placeholder = form.Placeholder
        });
    }
}