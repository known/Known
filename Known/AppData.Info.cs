namespace Known;

/// <summary>
/// 框架配置数据信息类。
/// </summary>
public class AppDataInfo
{
    /// <summary>
    /// 取得或设置语言信息列表。
    /// </summary>
    public List<LanguageInfo> Languages { get; set; } = [];

    /// <summary>
    /// 取得或设置按钮信息列表。
    /// </summary>
    public List<ButtonInfo> Buttons { get; set; } = [];

    /// <summary>
    /// 取得或设置顶部导航信息列表。
    /// </summary>
    public List<PluginInfo> TopNavs { get; set; } = [];

    /// <summary>
    /// 取得或设置模块信息列表。
    /// </summary>
    public List<ModuleInfo> Modules { get; set; } = [];
}