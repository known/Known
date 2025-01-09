namespace Known.Weixins;

/// <summary>
/// 微信模板消息业务类。
/// </summary>
public class WeixinTemplateInfo
{
    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置模板消息信息。
    /// </summary>
    public TemplateInfo Template { get; set; }
}