namespace Known;

/// <summary>
/// 框架插件信息类。
/// </summary>
public class PluginInfo
{
    /// <summary>
    /// 取得或设置插件实例ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置插件组件类型全名。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置插件组件参数配置JSON。
    /// </summary>
    public string Setting { get; set; }
}