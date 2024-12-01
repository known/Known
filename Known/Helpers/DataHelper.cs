using AntDesign;

namespace Known.Helpers;

/// <summary>
/// 数据帮助者类。
/// </summary>
public sealed class DataHelper
{
    private DataHelper() { }

    /// <summary>
    /// 取得系统配置的模块列表。
    /// </summary>
    public static List<ModuleInfo> Modules { get; private set; } = [];

    /// <summary>
    /// 取得实体模型列表。
    /// </summary>
    public static List<EntityInfo> Models { get; } = [];

    /// <summary>
    /// 取得流程模型列表。
    /// </summary>
    public static List<FlowInfo> Flows { get; } = [];

    #region Module
    /// <summary>
    /// 初始化模块数据。
    /// </summary>
    /// <param name="modules">系统模块列表。</param>
    public static void Initialize(List<ModuleInfo> modules)
    {
        if (modules == null || modules.Count == 0)
            return;

        Modules = modules;
        Models.Clear();
        var models = modules.Where(m => !string.IsNullOrWhiteSpace(m.EntityData) && m.EntityData.Contains('|')).Select(m => m.EntityData).ToList();
        foreach (var item in models)
        {
            var model = GetEntityInfo(item);
            Models.Add(model);
        }

        Flows.Clear();
        var flows = modules.Where(m => !string.IsNullOrWhiteSpace(m.FlowData) && m.FlowData.Contains('|')).Select(m => m.FlowData).ToList();
        foreach (var item in flows)
        {
            var flow = GetFlowInfo(item);
            Flows.Add(flow);
        }
    }

    /// <summary>
    /// 根据ID获取模块信息。
    /// </summary>
    /// <param name="id">模块ID。</param>
    /// <returns>模块信息。</returns>
    public static ModuleInfo GetModule(string id)
    {
        return Modules.FirstOrDefault(m => m.Id == id);
    }

    /// <summary>
    /// 获取路由模块。
    /// </summary>
    /// <param name="language">多语言对象。</param>
    /// <param name="moduleUrls">模块列表。</param>
    public static List<ModuleInfo> GetRouteModules(Language language, List<string> moduleUrls)
    {
        var routes = Config.RouteTypes;
        if (routes.Count == 0)
            return null;

        var modules = new List<ModuleInfo>();
        var routeError = typeof(ErrorPage).RouteTemplate();
        var routeAuto = typeof(AutoTablePage).RouteTemplate();
        var target = Constants.Route;
        var route = new ModuleInfo { Id = "route", Name = language["Route"], Target = target, Icon = "share-alt", ParentId = "0", Enabled = true, Sort = moduleUrls.Count + 1 };
        modules.Add(route);
        foreach (var item in routes.OrderBy(r => r.Key))
        {
            if (moduleUrls.Exists(m => m == item.Key) ||
                UIConfig.IgnoreRoutes.Contains(item.Key) ||
                item.Key == routeError || item.Key == routeAuto)
                continue;

            var parentId = route.Id;
            var index = item.Key.TrimStart('/').IndexOf('/');
            if (index > 0)
            {
                var key = item.Key.Substring(0, index + 1);
                var id = $"sub_{key}";
                var sub = modules.FirstOrDefault(m => m.Id == id);
                if (sub == null)
                {
                    sub = new ModuleInfo { Id = id, Name = key, Target = target, Icon = "folder", ParentId = route.Id, Enabled = true };
                    modules.Add(sub);
                }
                parentId = sub.Id;
            }

            var tab = item.Value.GetCustomAttribute<ReuseTabsPageAttribute>();
            var name = tab?.Title ?? item.Key;
            modules.Add(new ModuleInfo { Id = item.Value.FullName, Name = name, Url = item.Key, Target = target, Icon = "file", ParentId = parentId, Enabled = true });
        }
        return modules;
    }
    #endregion

    #region Entity
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

    /// <summary>
    /// 根据模块ID获取实体信息。
    /// </summary>
    /// <param name="moduleId">模块ID。</param>
    /// <returns>实体信息。</returns>
    public static EntityInfo GetEntityByModuleId(string moduleId)
    {
        var module = Modules.FirstOrDefault(m => m.Id == moduleId);
        return ToEntity(module?.EntityData);
    }

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
    /// <param name="tableName">数据表名。</param>
    /// <param name="model">字典对象。</param>
    /// <returns>验证结果。</returns>
    public static Result Validate(Context context, string tableName, Dictionary<string, object> model)
    {
        var entity = Models.FirstOrDefault(m => m.Id == tableName);
        if (entity == null)
            return Result.Error(context.Language.Required(tableName));

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
    #endregion
}