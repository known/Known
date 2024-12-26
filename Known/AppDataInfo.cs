namespace Known;

/// <summary>
/// 框架配置数据信息类。
/// </summary>
public partial class AppDataInfo
{
    /// <summary>
    /// 取得或设置顶部导航信息列表。
    /// </summary>
    public List<TopNavInfo> TopNavs { get; set; } = [];

    /// <summary>
    /// 取得或设置模块信息列表。
    /// </summary>
    public List<ModuleInfo> Modules { get; set; } = [];
}

/// <summary>
/// 顶部导航信息类。
/// </summary>
public class TopNavInfo
{
    /// <summary>
    /// 取得或设置插件ID。
    /// </summary>
    public string PluginId { get; set; }

    /// <summary>
    /// 取得或设置插件参数JSON。
    /// </summary>
    public string Parameters { get; set; }
}