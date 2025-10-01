namespace Known;

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