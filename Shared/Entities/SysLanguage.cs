namespace Known.Entities;

/// <summary>
/// 语言信息实体类。
/// </summary>
[DisplayName("语言信息")]
public class SysLanguage : EntityBase
{
    /// <summary>
    /// 取得或设置简体中文。
    /// </summary>
    [MaxLength(200)]
    [DisplayName("简体中文")]
    public string Chinese { get; set; }

    /// <summary>
    /// 取得或设置繁体中文。
    /// </summary>
    [MaxLength(200)]
    [DisplayName("繁体中文")]
    public string ChineseT { get; set; }

    /// <summary>
    /// 取得或设置英文。
    /// </summary>
    [MaxLength(200)]
    [DisplayName("英文")]
    public string English { get; set; }

    /// <summary>
    /// 取得或设置语言1。
    /// </summary>
    [MaxLength(200)]
    [DisplayName("语言1")]
    public string Language1 { get; set; }

    /// <summary>
    /// 取得或设置语言2。
    /// </summary>
    [MaxLength(200)]
    [DisplayName("语言2")]
    public string Language2 { get; set; }

    /// <summary>
    /// 取得或设置语言3。
    /// </summary>
    [MaxLength(200)]
    [DisplayName("语言3")]
    public string Language3 { get; set; }

    /// <summary>
    /// 取得或设置语言4。
    /// </summary>
    [MaxLength(200)]
    [DisplayName("语言4")]
    public string Language4 { get; set; }

    /// <summary>
    /// 取得或设置语言5。
    /// </summary>
    [MaxLength(200)]
    [DisplayName("语言5")]
    public string Language5 { get; set; }
}