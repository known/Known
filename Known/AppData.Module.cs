namespace Known;

/// <summary>
/// 框架模块信息类。
/// </summary>
public partial class ModuleInfo
{
    /// <summary>
    /// 取得或设置布局信息。
    /// </summary>
    public LayoutInfo Layout { get; set; }

    /// <summary>
    /// 取得或设置插件配置信息列表。
    /// </summary>
    public List<PluginInfo> Plugins { get; set; } = [];
}