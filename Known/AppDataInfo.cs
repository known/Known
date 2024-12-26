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

/// <summary>
/// 系统模块配置信息类。
/// </summary>
public class ModuleInfo1
{
    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置上级。
    /// </summary>
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置图标。
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置类型（Menu/Page/Link）。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置目标（None/Blank/IFrame）。
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置Url地址。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置插件ID。
    /// </summary>
    public string PluginId { get; set; }

    /// <summary>
    /// 取得或设置插件参数JSON。
    /// </summary>
    public string Parameters { get; set; }
}