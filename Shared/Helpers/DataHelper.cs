namespace Known.Helpers;

/// <summary>
/// 数据帮助者类。
/// </summary>
public sealed class DataHelper
{
    private DataHelper() { }

    /// <summary>
    /// 根据实体表名获取去表前缀的类名称。
    /// </summary>
    /// <param name="name">实体表名。</param>
    /// <returns>去表前缀的类名称。</returns>
    public static string GetClassName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var index = name.Select((c, i) => new { Char = c, Index = i })
                        .Where(x => char.IsUpper(x.Char))
                        .Skip(1).Select(x => x.Index)
                        .DefaultIfEmpty(-1).First();
        if (index <= 0)
            return name;

        return name.Substring(index);
    }

    #region Module
    /// <summary>
    /// 初始化模块数据，提取模块中的实体模型和流程模型数据。
    /// </summary>
    /// <param name="modules">系统模块列表。</param>
    public static void Initialize(List<ModuleInfo> modules)
    {
        if (modules == null || modules.Count == 0)
            return;

        //AppData.Data.Modules = modules;
        Models.Clear();
        FlowHelper.Flows.Clear();
        foreach (var item in modules)
        {
            var param = item.Plugins?.GetPluginParameter<AutoPageInfo>();
            if (param == null)
                continue;

            if (!string.IsNullOrWhiteSpace(param.EntityData) && param.EntityData.Contains('|'))
            {
                var model = GetEntityInfo(param.EntityData);
                Models.Add(model);
            }
            if (!string.IsNullOrWhiteSpace(param.FlowData) && param.FlowData.Contains('|'))
            {
                var flow = FlowHelper.GetFlowInfo(param.FlowData);
                FlowHelper.Flows.Add(flow);
            }
        }
    }

    /// <summary>
    /// 获取所有菜单信息列表。
    /// </summary>
    /// <returns></returns>
    public static async Task<List<MenuInfo>> GetMenusAsync(Database db = null)
    {
        var menus = CoreConfig.OnInitialMenus != null ? await CoreConfig.OnInitialMenus.Invoke(db) : [];
        menus ??= [];
        return GetMenus(menus);
    }

    internal static List<MenuInfo> GetMenus(List<MenuInfo> menus)
    {
        // 定义新列表，在新列表中添加路由，不污染原列表
        var allMenus = new List<MenuInfo>();
        if (menus != null && menus.Count > 0)
            allMenus.AddRange(menus);
        foreach (var item in Config.Modules)
        {
            if (!allMenus.Exists(m => m.Name == item.Name && m.ParentId == item.ParentId))
                allMenus.Add(item);
        }
        AddRules(allMenus);
        AddRoles(allMenus);
        //var routes = GetRouteModules([.. modules.Select(m => m.Url)]);
        //if (routes != null && routes.Count > 0)
        //    allModules.AddRange(routes);
        return [.. allMenus.Where(m => m.Enabled).OrderBy(m => m.Sort)];
    }

    internal static List<MenuInfo> Routes { get; } = [];
    private static void AddRules(List<MenuInfo> modules)
    {
        if (Routes.Count == 0)
            return;

        var items = Routes.Where(d => !modules.Exists(m => m.Id == d.Id || m.Url == d.Url)).ToList();
        var exists = Routes.Where(d => modules.Exists(m => m.Id == d.Id || m.Url == d.Url)).ToList();
        if (exists != null && exists.Count > 0)
        {
            foreach (var item in exists)
            {
                if (!item.IsCode)
                    continue;

                var info = modules.FirstOrDefault(m => m.Id == item.Id || m.Url == item.Url);
                info.Plugins = item.Plugins;
            }
        }

        if (items != null && items.Count > 0)
            modules.AddRange(items);
    }

    internal static List<MenuInfo> Roles { get; } = [];
    private static void AddRoles(List<MenuInfo> modules)
    {
        if (Roles.Count == 0)
            return;

        var items = Roles.Where(d => !modules.Exists(m => m.Id == d.Id)).ToList();
        if (items != null && items.Count > 0)
            modules.AddRange(items);
    }

    //private static List<ModuleInfo> GetRouteModules(List<string> moduleUrls)
    //{
    //    var routes = Config.RouteTypes;
    //    if (routes.Count == 0)
    //        return null;

    //    var infos = new List<ModuleInfo>();
    //    var routeError = typeof(ErrorPage).RouteTemplate();
    //    var routeAuto = typeof(AutoPage).RouteTemplate();
    //    var target = Constants.Route;
    //    var route = new ModuleInfo { Id = "route", ParentId = "0", Name = "路由", Target = target, Icon = "share-alt", Sort = 999 };
    //    foreach (var item in routes.OrderBy(r => r.Key))
    //    {
    //        if (moduleUrls.Exists(m => m == item.Key) ||
    //            UIConfig.IgnoreRoutes.Contains(item.Key) ||
    //            item.Key.StartsWith("/dev") ||
    //            item.Key == routeError || item.Key == routeAuto)
    //            continue;

    //        var parentId = route.Id;
    //        var index = item.Key.TrimStart('/').IndexOf('/');
    //        if (index > 0)
    //        {
    //            var key = item.Key.Substring(0, index + 1);
    //            var id = $"sub_{key}";
    //            var sub = infos.FirstOrDefault(m => m.Id == id);
    //            if (sub == null)
    //            {
    //                sub = new ModuleInfo { Id = id, ParentId = route.Id, Name = key, Target = target, Icon = "folder" };
    //                infos.Add(sub);
    //            }
    //            parentId = sub.Id;
    //        }

    //        var module = GetModule(item, parentId);
    //        infos.Add(module);
    //    }

    //    var modules = new List<ModuleInfo>();
    //    if (infos.Count > 0)
    //    {
    //        modules.Add(route);
    //        modules.AddRange(infos);
    //    }
    //    return modules;
    //}

    //private static ModuleInfo GetModule(KeyValuePair<string, Type> item, string parentId)
    //{
    //    var info = new ModuleInfo
    //    {
    //        Id = item.Value.FullName,
    //        ParentId = parentId,
    //        Name = item.Value.Name,
    //        Url = item.Key,
    //        Target = Constants.Route,
    //        Icon = "file",
    //        Enabled = true
    //    };
    //    SetRouteInfo(info, item.Value);
    //    info.AddActions(item.Value);
    //    return info;
    //}

    //private static void SetRouteInfo(ModuleInfo info, Type type)
    //{
    //    var tab = type.GetCustomAttribute<ReuseTabsPageAttribute>();
    //    if (tab != null)
    //    {
    //        info.Name = tab.Title;
    //        return;
    //    }

    //    var plugin = type.GetCustomAttribute<PluginAttribute>();
    //    if (plugin != null)
    //    {
    //        info.Name = plugin.Name;
    //        info.Icon = plugin.Icon;
    //        return;
    //    }

    //    var menu = type.GetCustomAttribute<MenuAttribute>();
    //    if (menu != null)
    //    {
    //        info.Name = menu.Name;
    //        info.Icon = menu.Icon;
    //    }
    //}
    #endregion

    #region Entity
    /// <summary>
    /// 取得实体模型列表。
    /// </summary>
    public static List<EntityInfo> Models { get; } = [];

    /// <summary>
    /// 将模型配置转换成实体信息对象。
    /// </summary>
    /// <param name="model">模型字符串。</param>
    /// <returns></returns>
    public static EntityInfo ToEntity(string model)
    {
        var info = new EntityInfo();
        if (string.IsNullOrWhiteSpace(model))
            return info;

        if (!model.Contains('|'))
            return Models.FirstOrDefault(m => m.Id == model);

        return GetEntityInfo(model);
    }

    // 验证无代码字典对象。
    internal static Result Validate(Context context, EntityInfo entity, Dictionary<string, object> model)
    {
        if (entity == null)
            return Result.Error(context.Language.Required("EntityInfo"));

        var dicError = new Dictionary<string, List<string>>();
        foreach (var field in entity.Fields)
        {
            var errors = new List<string>();
            var value = model.GetValue(field.Id);
            if (value == null && (field.Type == FieldType.Switch || field.Type == FieldType.CheckBox))
            {
                value = "False";
                model.SetValue(field.Id, false);
            }

            var valueString = value == null ? "" : value.ToString().Trim();
            if (field.Required && string.IsNullOrWhiteSpace(valueString))
                errors.Add(context.Language.Required(field.Name));

            if (errors.Count > 0)
                dicError.Add(field.Name, errors);
        }

        if (dicError.Count > 0)
        {
            var result = Result.Error("", dicError);
            foreach (var item in dicError.Values)
            {
                item.ForEach(result.AddError);
            }
            return result;
        }

        return Result.Success("");
    }

    internal static EntityInfo GetEntityInfo(string model)
    {
        var info = new EntityInfo();
        var lines = model.Split([.. Environment.NewLine])
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .ToArray();

        if (lines.Length > 0)
        {
            var values = lines[0].Split('|').Select(x => x.Trim()).ToArray();
            if (values.Length > 0) info.Name = values[0];
            if (values.Length > 1) info.Id = values[1];
            if (values.Length > 2) info.IsFlow = values[2] == "Y";
        }

        if (lines.Length > 1)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                var field = new FieldInfo();
                var values = lines[i].Split('|').Select(x => x.Trim()).ToArray();
                if (values.Length > 0) field.Name = values[0];
                if (values.Length > 1) field.Id = values[1];
                if (values.Length > 2) field.Type = Utils.ConvertTo<FieldType>(values[2]);
                if (values.Length > 3) field.Length = values[3];
                if (values.Length > 4) field.Required = values[4] == "Y";

                if (field.Type == FieldType.CheckBox || field.Type == FieldType.Switch)
                {
                    field.Length = "50";
                    field.Required = true;
                }

                info.Fields.Add(field);
            }
        }

        return info;
    }

    /// <summary>
    /// 将实体信息对象转换成模型配置字符串。
    /// </summary>
    /// <param name="info">实体信息对象。</param>
    /// <returns></returns>
    public static string ToEntityData(EntityInfo info)
    {
        if (info == null)
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine($"{info.Name}|{info.Id}");
        foreach (var item in info.Fields)
        {
            sb.AppendLine($"{item.Name}|{item.Id}|{item.Type}|{item.Length}|{(item.Required ? "Y" : "N")}");
        }
        return sb.ToString();
    }
    #endregion

    #region Dictionary
    //internal static Task<PagingResult<Dictionary<string, object>>> QueryPrototypeDataAsync(PagingCriteria criteria, MenuInfo info)
    //{
    //    var columns = info?.GetAutoPageParameter()?.Page?.Columns;
    //    if (columns == null || columns.Count == 0)
    //        return Task.FromResult(new PagingResult<Dictionary<string, object>>());

    //    var datas = new List<Dictionary<string, object>>();
    //    for (int i = 0; i < 100; i++)
    //    {
    //        var data = new Dictionary<string, object>();
    //        foreach (var column in columns)
    //        {
    //            if (UIConfig.OnMockData != null)
    //                data[column.Id] = UIConfig.OnMockData.Invoke(info, column);
    //            else
    //                data[column.Id] = "TestData";
    //        }
    //        datas.Add(data);
    //    }
    //    var result = datas.ToPagingResult(criteria);
    //    return Task.FromResult(result);
    //}
    #endregion
}