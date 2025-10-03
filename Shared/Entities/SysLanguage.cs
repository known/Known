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
    [Required]
    [MaxLength(200)]
    [Language("zh-CN", "简", true, true)]
    [Column(IsQuery = true, IsViewLink = true)]
    [DisplayName("简体中文")]
    public string Chinese { get; set; }

    /// <summary>
    /// 取得或设置繁体中文。
    /// </summary>
    [MaxLength(200)]
    [Language("zh-TW", "繁", false, true)]
    [DisplayName("繁体中文")]
    public string ChineseT { get; set; }

    /// <summary>
    /// 取得或设置英文。
    /// </summary>
    [MaxLength(200)]
    [Language("en-US", "EN", false, true)]
    [DisplayName("English")]
    public string English { get; set; }

    /// <summary>
    /// 取得或设置语言1。
    /// </summary>
    [MaxLength(200)]
    [Language("Lang1", "L1")]
    [DisplayName("语言1")]
    public string Language1 { get; set; }

    /// <summary>
    /// 取得或设置语言2。
    /// </summary>
    [MaxLength(200)]
    [Language("Lang2", "L2")]
    [DisplayName("语言2")]
    public string Language2 { get; set; }

    /// <summary>
    /// 取得或设置语言3。
    /// </summary>
    [MaxLength(200)]
    [Language("Lang3", "L3")]
    [DisplayName("语言3")]
    public string Language3 { get; set; }

    /// <summary>
    /// 取得或设置语言4。
    /// </summary>
    [MaxLength(200)]
    [Language("Lang4", "L4")]
    [DisplayName("语言4")]
    public string Language4 { get; set; }

    /// <summary>
    /// 取得或设置语言5。
    /// </summary>
    [MaxLength(200)]
    [Language("Lang5", "L5")]
    [DisplayName("语言5")]
    public string Language5 { get; set; }
}