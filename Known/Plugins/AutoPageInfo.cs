namespace Known.Plugins;

/// <summary>
/// 自动页面插件配置信息类。
/// </summary>
public class AutoPageInfo
{
    /// <summary>
    /// 取得或设置页面ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置页面名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置页面关联单表实体ID。
    /// </summary>
    public string EntityId { get; set; }

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