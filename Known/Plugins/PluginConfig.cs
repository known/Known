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
    /// 取得或设置插件ID转换委托，适用于插件组件类名变更。
    /// </summary>
    public static Func<string, string> OnPluginIdTrans { get; set; }

    internal static List<PluginMenuInfo> TopNavs => [.. Plugins.Where(p => p.IsNavComponent).OrderBy(p => p.Sort)];
    internal static List<PluginMenuInfo> NavPlugins => [.. Plugins.Where(p => p.IsNav)];
    internal static List<PluginMenuInfo> DevPlugins => [.. Plugins.Where(p => p.IsDev).OrderBy(p => p.Sort)];
    internal static List<PluginMenuInfo> PagePlugins => [.. Plugins.Where(p => p.IsPage)];

    internal static PluginMenuInfo GetPlugin(string id)
    {
        if (OnPluginIdTrans != null)
            id = OnPluginIdTrans.Invoke(id);
        return Plugins.FirstOrDefault(p => p.Id == id);
    }

    internal static void AddPlugin(Type item, PluginAttribute plugin, RouteAttribute route)
    {
        if (plugin.Name == Language.NavFontSize && !Config.App.IsSize)
            return;
        if (plugin.Name == Language.NavLanguage && !Config.App.IsLanguage)
            return;
        if (plugin.Name == Language.NavTheme && !Config.App.IsTheme)
            return;

        Plugins.Add(new PluginMenuInfo(item, plugin)
        {
            Url = route?.Template
        });
    }
}