namespace Known.Extensions;

/// <summary>
/// 插件扩展类。
/// </summary>
public static class PluginExtension
{
    /// <summary>
    /// 获取自动页面插件参数配置信息。
    /// </summary>
    /// <param name="menu">页面菜单信息。</param>
    /// <returns></returns>
    public static AutoPageInfo GetAutoPageParameter(this MenuInfo menu)
    {
        // 页面默认只有一个插件
        var plugin = menu?.Plugins?.FirstOrDefault();
        if (plugin == null)
            return null;

        return Utils.FromJson<AutoPageInfo>(plugin.Setting);
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
            if (string.IsNullOrWhiteSpace(id))
                id = Utils.GetNextId();
            plugin = new PluginInfo { Id = id, Type = type };
            plugins.Add(plugin);
        }
        plugin.Setting = Utils.ToJson(param);
    }
}