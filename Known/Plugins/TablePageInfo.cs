namespace Known.Plugins;

/// <summary>
/// 单表实体页面插件配置信息类。
/// </summary>
public class TablePageInfo
{
    /// <summary>
    /// 取得或设置实体设置。
    /// </summary>
    public string EntityData { get; set; }

    /// <summary>
    /// 取得或设置流程设置。
    /// </summary>
    public string FlowData { get; set; }

    /// <summary>
    /// 取得或设置无代码页面配置信息。
    /// </summary>
    public PageInfo Page { get; set; } = new();

    /// <summary>
    /// 取得或设置无代码表单配置信息。
    /// </summary>
    public FormInfo Form { get; set; } = new();
}