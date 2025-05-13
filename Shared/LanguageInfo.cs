namespace Known;

/// <summary>
/// 语言信息信息类。
/// </summary>
[DisplayName("语言信息")]
public class LanguageInfo
{
    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置简体中文。
    /// </summary>
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

/// <summary>
/// 系统语言设置信息类。
/// </summary>
public class LanguageSettingInfo
{
    /// <summary>
    /// 取得或设置语言ID。
    /// </summary>
    [Form, Required]
    [Column(Width = 120)]
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置语言ID。
    /// </summary>
    [Form, Required]
    [Column(Width = 120)]
    [DisplayName("字段")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置语言名称。
    /// </summary>
    [Form, Required]
    [Column(IsQuery = true, Width = 120)]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置语言图标。
    /// </summary>
    [Form, Required]
    [Column(Width = 120)]
    [DisplayName("图标")]
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置语言是否默认。
    /// </summary>
    [Required]
    [Column, Form]
    [DisplayName("默认")]
    public bool Default { get; set; }

    /// <summary>
    /// 取得或设置语言是否启用。
    /// </summary>
    [Required]
    [Column, Form]
    [DisplayName("启用")]
    public bool Enabled { get; set; }
}