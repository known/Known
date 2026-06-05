namespace Known.Plugins;

/// <summary>
/// 插件全局配置类。
/// </summary>
public class PluginConfig
{
    private PluginConfig() { }

    /// <summary>
    /// 取得框架插件信息列表。
    /// </summary>
    public static List<PluginMenuInfo> Plugins { get; } = [];

    /// <summary>
    /// 取得框架插件忽略的插件类型列表。
    /// </summary>
    public static List<Type> IgnoreTypes { get; } = [];

    internal static List<Type> Excludes { get; } = [];
    internal static List<PluginMenuInfo> TopNavs => [.. Plugins.Where(p => p.IsNavComponent).OrderBy(p => p.Sort)];
    internal static List<PluginMenuInfo> NavPlugins => [.. Plugins.Where(p => p.IsNav)];
    internal static List<PluginMenuInfo> DevPlugins => [.. Plugins.Where(p => p.IsDev).OrderBy(p => p.Sort)];
    internal static List<PluginMenuInfo> PagePlugins => [.. Plugins.Where(p => p.IsPage)];

    /// <summary>
    /// 排除框架内置插件。
    /// </summary>
    /// <typeparam name="T">插件类型。</typeparam>
    public static void Remove<T>()
    {
        var type = typeof(T);
        if (Excludes.Contains(type))
            return;

        Excludes.Add(type);
    }

    internal static bool IsExclude(string typeName)
    {
        return Excludes.Any(t => t.FullName == typeName);
    }

    internal static PluginMenuInfo GetPlugin(string id)
    {
        if (id == typeof(AutoTablePlugin).FullName)
            id = typeof(NoCodeTablePlugin).FullName;
        return Plugins.FirstOrDefault(p => p.Id == id);
    }

    internal static void AddPlugin(Type item, PluginAttribute plugin, RouteAttribute route)
    {
        if (Config.App.Type == AppType.Desktop && plugin.Name == "WebApi")
            return;

        if (Excludes.Contains(item))
            return;

        Language.DefaultDatas.Add(plugin.Name);

        if (plugin.Name == Language.DevTenant && !Config.App.IsPlatform)
            return;
        if (plugin.Name == Language.NavFontSize && !Config.App.IsSize)
            return;
        if (plugin.Name == Language.NavLanguage && !Config.App.IsLanguage)
            return;
        if (plugin.Name == Language.NavTheme && !Config.App.IsTheme)
            return;
        if (plugin.Name == Language.NavUser && Config.App.Layout == LayoutType.Side)
            return;
        if (IgnoreTypes.Contains(item))
            return;

        Plugins.Add(new PluginMenuInfo(item, plugin) { Url = route?.Template });
    }

    // 加载顶部导航
    internal static List<PluginInfo> LoadTopNavs()
    {
        var infos = new List<PluginInfo>();
        foreach (var item in TopNavs)
        {
            if (item.Type == typeof(NavFontSize) && !Config.App.IsSize)
                continue;
            if (item.Type == typeof(NavLanguage) && !Config.App.IsLanguage)
                continue;
            if (item.Type == typeof(NavTheme) && !Config.App.IsTheme)
                continue;
            if (item.Type == typeof(NavUser) && Config.App.Layout == LayoutType.Side)
                continue;
            infos.Add(new PluginInfo { Id = item.Id, Type = item.Id });
        }
        return infos;
    }
}