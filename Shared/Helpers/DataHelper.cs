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
    public static void Initialize(List<SysModule> modules)
    {
        if (modules == null || modules.Count == 0)
            return;

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
        var menus = new List<MenuInfo>();
        var items = await db.QueryListAsync<SysModule>();
        if (items != null && items.Count > 0)
        {
            foreach (var item in items.OrderBy(m => m.Sort))
            {
                menus.Add(item.ToMenuInfo());
            }
        }
        return GetMenus(menus);
    }

    internal static List<MenuInfo> GetMenus(List<MenuInfo> menus, bool isRoute = true)
    {
        // 定义新列表，在新列表中添加路由，不污染原列表
        var allMenus = new List<MenuInfo>();
        if (menus != null && menus.Count > 0)
            allMenus.AddRange(menus);
        foreach (var item in Config.Modules)
        {
            if (!allMenus.Exists(m => m.Name == item.Name && m.ParentId == item.ParentId))
                allMenus.Add(item.Clone(!isRoute));
        }
        AddRoutes(allMenus, isRoute);
        return [.. allMenus.Where(m => m.Enabled).OrderBy(m => m.Sort)];
    }

    /// <summary>
    /// 取得初始化加载的路由组件菜单列表。
    /// </summary>
    public static ConcurrentBag<MenuInfo> Routes { get; } = [];

    private static void AddRoutes(List<MenuInfo> menus, bool isRoute = true)
    {
        if (Routes.IsEmpty)
            return;

        var routes = new List<MenuInfo>();
        foreach (var item in Routes)
        {
            if (isRoute || item.Target != Constants.Route)
                routes.Add(item.Clone());
        }
        var items = routes.Where(d => !menus.Exists(m => m.Id == d.Id || m.Url == d.Url)).ToList();
        var exists = routes.Where(d => menus.Exists(m => m.Id == d.Id || m.Url == d.Url)).ToList();
        if (exists != null && exists.Count > 0)
        {
            foreach (var item in exists)
            {
                if (!item.IsCode)
                    continue;

                var info = menus.FirstOrDefault(m => m.Id == item.Id || m.Url == item.Url);
                info.Plugins = item.Plugins;
            }
        }

        if (items != null && items.Count > 0)
            menus.AddRange(items);
    }
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
}