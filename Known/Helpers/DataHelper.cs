namespace Known.Helpers;

/// <summary>
/// 数据帮助者类。
/// </summary>
public sealed class DataHelper
{
    private DataHelper() { }

    /// <summary>
    /// 取得实体模型列表。
    /// </summary>
    public static List<EntityInfo> Models { get; } = [];

    /// <summary>
    /// 取得流程模型列表。
    /// </summary>
    public static List<FlowInfo> Flows { get; } = [];

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
        Flows.Clear();
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
                var flow = GetFlowInfo(param.FlowData);
                Flows.Add(flow);
            }
        }
    }

    /// <summary>
    /// 获取所有模块信息列表。
    /// </summary>
    /// <returns></returns>
    public static async Task<List<ModuleInfo>> GetModulesAsync(Database db = null)
    {
        var modules = Config.OnInitialModules != null ? await Config.OnInitialModules.Invoke(db) : [];
        modules ??= [];
        //if (!Config.IsDbMode)
        //    modules.Add(AppData.Data.Modules);
        //if (modules.Count == 0)
        //    modules = AppData.Data.Modules;
        return GetModules(modules);
    }

    /// <summary>
    /// 获取所有新模块信息实例列表。
    /// </summary>
    /// <param name="modules">原模块信息列表。</param>
    /// <returns>新模块信息列表。</returns>
    public static List<ModuleInfo> GetModules(List<ModuleInfo> modules)
    {
        // 定义新列表，在新列表中添加路由模块，不污染原模块列表
        var allModules = new List<ModuleInfo>();
        if (modules != null && modules.Count > 0)
            allModules.AddRange(modules);
        RouteHelper.AddTo(allModules);
        RoleHelper.AddTo(allModules);
        //var routes = GetRouteModules([.. modules.Select(m => m.Url)]);
        //if (routes != null && routes.Count > 0)
        //    allModules.AddRange(routes);
        return [.. allModules.Where(m => m.Enabled).OrderBy(m => m.Sort)];
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
    /// 将模型配置转换成实体信息对象。
    /// </summary>
    /// <param name="model">模型配置。</param>
    /// <returns>实体信息。</returns>
    public static EntityInfo ToEntity(string model)
    {
        var info = new EntityInfo();
        if (string.IsNullOrWhiteSpace(model))
            return info;

        if (!model.Contains('|'))
            return Models.FirstOrDefault(m => m.Id == model);

        return GetEntityInfo(model);
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

    private static EntityInfo GetEntityInfo(string model)
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
    #endregion

    #region Flow
    /// <summary>
    /// 将流程配置转换成流程信息对象。
    /// </summary>
    /// <param name="model">流程配置。</param>
    /// <returns>流程信息。</returns>
    public static FlowInfo ToFlow(string model)
    {
        var info = new FlowInfo();
        if (string.IsNullOrWhiteSpace(model))
            return info;

        if (!model.Contains('|'))
            return Flows.FirstOrDefault(m => m.Id == model);

        return GetFlowInfo(model);
    }

    private static FlowInfo GetFlowInfo(string model)
    {
        var info = new FlowInfo();
        var lines = model.Split([.. Environment.NewLine])
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .ToArray();

        if (lines.Length > 0)
        {
            var values = lines[0].Split('|');
            if (values.Length > 0) info.Name = values[0];
            if (values.Length > 1) info.Id = values[1];
        }

        if (lines.Length > 1)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                var step = new FlowStepInfo();
                var values = lines[i].Split('|');
                if (values.Length > 0) step.Name = values[0];
                if (values.Length > 1) step.Id = values[1];
                if (values.Length > 2) step.User = values[2];
                if (values.Length > 3) step.Role = values[3];
                if (values.Length > 4) step.Pass = values[4];
                if (values.Length > 5) step.Fail = values[5];
                info.Steps.Add(step);
            }
        }

        return info;
    }
    #endregion

    #region Dictionary
    /// <summary>
    /// 验证无代码字典对象。
    /// </summary>
    /// <param name="context">系统上下文。</param>
    /// <param name="entity">数据实体信息。</param>
    /// <param name="model">字典对象。</param>
    /// <returns>验证结果。</returns>
    public static Result Validate(Context context, EntityInfo entity, Dictionary<string, object> model)
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

    internal static Task<PagingResult<Dictionary<string, object>>> QueryPrototypeDataAsync(PagingCriteria criteria, MenuInfo info)
    {
        var columns = info?.TablePage?.Page?.Columns;
        if (columns == null || columns.Count == 0)
            return Task.FromResult(new PagingResult<Dictionary<string, object>>());

        var datas = new List<Dictionary<string, object>>();
        for (int i = 0; i < 100; i++)
        {
            var data = new Dictionary<string, object>();
            foreach (var column in columns)
            {
                if (UIConfig.OnMockData != null)
                    data[column.Id] = UIConfig.OnMockData.Invoke(info, column);
                else
                    data[column.Id] = "TestData";
            }
            datas.Add(data);
        }
        var result = datas.ToPagingResult(criteria);
        return Task.FromResult(result);
    }
    #endregion
}