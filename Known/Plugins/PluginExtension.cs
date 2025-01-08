namespace Known.Plugins;

/// <summary>
/// 插件扩展类。
/// </summary>
public static class PluginExtension
{
    /// <summary>
    /// 获取表格页面插件参数配置信息。
    /// </summary>
    /// <param name="menu">页面菜单信息。</param>
    /// <returns></returns>
    public static TablePageInfo GetTablePageParameter(this MenuInfo menu)
    {
        // 表格页面默认只有一个插件
        var plugin = menu?.Plugins?.FirstOrDefault();
        if (plugin == null)
            return null;

        return Utils.FromJson<TablePageInfo>(plugin.Setting);
    }

    /// <summary>
    /// 获取插件信息。
    /// </summary>
    /// <param name="plugins">插件列表。</param>
    /// <param name="id">插件实例ID。</param>
    /// <returns></returns>
    public static PluginInfo GetPlugin(this List<PluginInfo> plugins, string id = null)
    {
        return !string.IsNullOrWhiteSpace(id)
               ? plugins?.FirstOrDefault(p => p.Id == id)
               : plugins?.FirstOrDefault();
    }

    /// <summary>
    /// 根据ID获取插件参数配置信息。
    /// </summary>
    /// <typeparam name="T">插件配置类型。</typeparam>
    /// <param name="plugins">插件列表。</param>
    /// <param name="id">插件实例ID。</param>
    /// <returns>插件配置信息。</returns>
    public static T GetPluginParameter<T>(this List<PluginInfo> plugins, string id = null)
    {
        var plugin = plugins.GetPlugin(id);
        if (plugin == null)
            return default;

        return Utils.FromJson<T>(plugin.Setting);
    }

    /// <summary>
    /// 添加插件配置信息。
    /// </summary>
    /// <typeparam name="T">插件参数配置类型。</typeparam>
    /// <param name="plugins">插件列表。</param>
    /// <param name="param">插件参数配置信息。</param>
    /// <param name="id">插件实例ID。</param>
    /// <param name="type">插件组件类型全名。</param>
    public static void AddPlugin<T>(this List<PluginInfo> plugins, T param, string id = null, string type = null)
    {
        var plugin = plugins.GetPlugin(id);
        if (plugin == null)
        {
            plugin = new PluginInfo { Id = id, Type = type };
            plugins.Add(plugin);
        }
        plugin.Setting = Utils.ToJson(param);
    }

    /// <summary>
    /// 构建插件组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="info">插件信息。</param>
    public static void BuildPlugin(this RenderTreeBuilder builder, PluginInfo info)
    {
        var plugin = Config.Plugins.FirstOrDefault(p => p.Id == info.Type);
        if (plugin == null)
            return;

        if (plugin.IsNavComponent)
        {
            builder.Component(plugin.Type);
            return;
        }

        builder.Component(plugin.Type, new Dictionary<string, object>
        {
            [nameof(IPlugin.Plugin)] = info
        });
    }

    // 插件菜单转下拉菜单项列表
    internal static List<ActionInfo> ToActions(this List<PluginMenuInfo> plugins)
    {
        var infos = new List<ActionInfo>();
        var categories = plugins.Where(p => !string.IsNullOrWhiteSpace(p.Attribute.Category))
                                .Select(p => p.Attribute.Category).Distinct().ToList();
        if (categories.Count > 0)
        {
            foreach (var category in categories)
            {
                var items = plugins.Where(p => p.Attribute.Category == category).OrderBy(p => p.Attribute.Sort);
                var info = new ActionInfo { Id = category, Icon = "folder", Name = category };
                info.Children.AddRange(items.Select(item => item.ToAction()));
                infos.Add(info);
            }
        }
        var others = plugins.Where(p => string.IsNullOrWhiteSpace(p.Attribute.Category)).OrderBy(p => p.Attribute.Sort);
        foreach (var item in others)
        {
            infos.Add(item.ToAction());
        }
        return infos;
    }
}