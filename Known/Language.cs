namespace Known;

public partial class Language
{
    internal const string TipFormRouteIsNull = "表单类型或路由不存在！";
    internal const string TipLanguageFetch = "提取系统语言常量、枚举、信息类、实体类字段名称。";
    internal const string TipLanguageSetting = "配置系统语言选项。";
    internal const string TipLanguageFetchConfirm = "确定要提取语言信息吗？";
    internal const string TipLanguageSettingConfirm = "确定要重置语言设置吗？";

    internal static void Initialize(Assembly assembly)
    {
        foreach (var item in Settings)
        {
            var content = Utils.GetResource(assembly, $"Locales.{item.Id}");
            if (string.IsNullOrWhiteSpace(content))
                continue;

            if (!caches.ContainsKey(item.Id))
                caches[item.Id] = [];

            var langs = Utils.FromJson<Dictionary<string, object>>(content);
            if (langs != null && langs.Count > 0)
            {
                foreach (var lang in langs)
                {
                    caches[item.Id][lang.Key] = lang.Value;
                }
            }
        }
    }

    /// <summary>
    /// 获取默认多语言信息列表。
    /// </summary>
    /// <returns>多语言信息列表。</returns>
    public static List<LanguageInfo> GetDefaultLanguages()
    {
        var infos = new List<LanguageInfo>();
        foreach (var item in Config.Modules)
        {
            infos.Add(item.Name);
        }
        foreach (var item in Config.Menus)
        {
            infos.Add(item.Name);
        }
        foreach (var item in Config.AppMenus)
        {
            infos.Add(item.Name);
        }
        foreach (var item in PluginConfig.Plugins)
        {
            infos.Add(item.Attribute?.Name);
        }
        foreach (var assembly in Config.Assemblies)
        {
            foreach (var item in assembly.GetTypes())
            {
                if (item.IsEnum)
                    infos.AddEnum(item);
                else if (item.IsAssignableTo(typeof(EntityBase)) || item.Name.EndsWith("Info"))
                    infos.AddAttribute(item);
                else if (item.Name.Contains(nameof(Language)))
                    infos.AddConstant(item);
            }
        }
        return infos;
    }

    /// <summary>
    /// 获取菜单语言。
    /// </summary>
    /// <param name="info">菜单信息对象。</param>
    /// <returns>菜单语言。</returns>
    public string GetString(MenuInfo info) => GetText("Menu", info?.Code, info?.Name);

    /// <summary>
    /// 获取操作按钮语言。
    /// </summary>
    /// <param name="info">操作按钮对象。</param>
    /// <returns>操作按钮语言。</returns>
    public string GetString(ActionInfo info) => GetText("Button", info?.Id, info?.Name);

    /// <summary>
    /// 获取字段语言。
    /// </summary>
    /// <param name="info">字段信息对象。</param>
    /// <param name="type">数据类型。</param>
    /// <returns>字段语言。</returns>
    public string GetString(ColumnInfo info, Type type = null) => GetText(type?.Name, info?.Id, info?.Name);

    /// <summary>
    /// 获取指定类型的字段语言。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="info">字段信息对象。</param>
    /// <returns>字段语言。</returns>
    public string GetString<T>(ColumnInfo info) => GetText(typeof(T).Name, info?.Id, info?.Name);
}