﻿namespace Known;

public partial class Config
{
    private static RenderType renderMode = RenderType.Server;
    /// <summary>
    /// 取得或设置呈现类型，默认Server。
    /// </summary>
    public static RenderType RenderMode
    {
        get { return renderMode; }
        set
        {
            renderMode = value;
            CurrentMode = value;
        }
    }

    /// <summary>
    /// 取得或设置当前呈现类型，默认Server。
    /// </summary>
    public static RenderType CurrentMode { get; set; } = RenderType.Server;

    /// <summary>
    /// 取得或设置系统移动端菜单信息列表。
    /// </summary>
    public static List<MenuInfo> AppMenus { get; set; } = [];

    /// <summary>
    /// 取得框架初始模块菜单信息列表。
    /// </summary>
    public static List<MenuAttribute> Menus { get; } = [];

    /// <summary>
    /// 取得或设置操作按钮信息列表。
    /// </summary>
    public static List<ActionInfo> Actions { get; set; } = [];

    /// <summary>
    /// 取得或设置获取初始化信息后附加操作委托。
    /// </summary>
    public static Action<InitialInfo> OnInitial { get; set; }

    /// <summary>
    /// 取得或设置获取Admin信息后附加操作委托。
    /// </summary>
    public static Action<AdminInfo> OnAdmin { get; set; }

    internal static void AddApp(Assembly assembly)
    {
        // 添加默认一级模块
        if (App.IsModule)
        {
            Modules.AddItem("0", Constants.BaseData, "基础数据", "database", 1);
            Modules.AddItem("0", Constants.System, "系统管理", "setting", 99);
        }

        Version = new VersionInfo(App.Assembly);
        InitAssembly(App.Assembly);
        AddModule(assembly);
    }

    private static void InitAssembly(Assembly assembly)
    {
        if (assembly == null)
            return;

        if (InitAssemblies.Contains(assembly.FullName))
            return;

        InitAssemblies.Add(assembly.FullName);
        AddActions(assembly);
        Language.Initialize(assembly);

        foreach (var item in assembly.GetTypes())
        {
            if (TypeHelper.IsGenericSubclass(item, typeof(EntityTablePage<>), out var arguments))
                AddApiMethod(typeof(IEntityService<>).MakeGenericType(arguments), item.Name);
            else if (item.IsInterface && !item.IsGenericTypeDefinition && item.IsAssignableTo(typeof(IService)) && item.Name != nameof(IService))
                AddApiMethod(item, item.Name[1..].Replace("Service", ""));
            else if (item.IsAssignableTo(typeof(ICustomField)))
                AddFieldType(item);
            else if (item.IsAssignableTo(typeof(BaseForm)))
                FormTypes[item.Name] = item;
            else if (item.IsEnum)
                Cache.AttachEnumCodes(item);

            var routes = GetRoutes(item);
            PluginConfig.AddPlugin(item, routes);
            AddAppMenu(item, routes);
            AddMenu(item, routes);
            AddCodeInfo(item);
        }
    }

    private static void AddActions(Assembly assembly)
    {
        var content = Utils.GetResource(assembly, "actions");
        if (string.IsNullOrWhiteSpace(content))
            return;

        var lines = content.Split([.. Environment.NewLine]);
        if (lines == null || lines.Length == 0)
            return;

        foreach (var item in lines)
        {
            if (string.IsNullOrWhiteSpace(item) || item.StartsWith("按钮编码"))
                continue;

            var values = item.Split('|');
            if (values.Length < 2)
                continue;

            var id = values[0].Trim();
            var info = Actions.FirstOrDefault(i => i.Id == id);
            if (info == null)
            {
                info = new ActionInfo { Id = id };
                Actions.Add(info);
            }
            if (values.Length > 1)
                info.Name = values[1].Trim();
            if (values.Length > 2)
                info.Icon = values[2].Trim();
            if (values.Length > 3)
                info.Style = values[3].Trim();
            if (values.Length > 4)
                info.Position = values[4].Trim();
        }
    }

    private static IEnumerable<RouteAttribute> GetRoutes(Type item)
    {
        var routes = item.GetCustomAttributes<RouteAttribute>();
        if (routes != null && routes.Any())
        {
            foreach (var route in routes)
            {
                RouteTypes[route.Template] = item;
            }
        }
        return routes;
    }

    private static void AddAppMenu(Type item, IEnumerable<RouteAttribute> routes)
    {
        var menu = item.GetCustomAttribute<AppMenuAttribute>();
        if (menu != null)
        {
            menu.Page = item;
            menu.Url = routes?.FirstOrDefault()?.Template;
            AppMenus.Add(new MenuInfo
            {
                Id = item.Name,
                Name = menu.Name,
                Icon = menu.Icon,
                Url = menu.Url,
                Sort = menu.Sort,
                Target = menu.Target,
                Role = menu.Role,
                Color = menu.Color,
                BackUrl = menu.BackUrl,
                PageType = item
            });
        }
    }

    private static void AddMenu(Type item, IEnumerable<RouteAttribute> routes)
    {
        var menu = item.GetCustomAttribute<MenuAttribute>();
        if (menu != null)
        {
            menu.Page = item;
            menu.Url = routes?.FirstOrDefault()?.Template;
            Menus.Add(menu);
        }
    }

    private static void AddFieldType(Type item)
    {
        if (item.Name == nameof(ICustomField) || item.Name == nameof(CustomField))
            return;

        FieldTypes[item.Name] = item;
    }
}