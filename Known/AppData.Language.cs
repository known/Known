namespace Known;

/// <summary>
/// 系统语言信息类。
/// </summary>
public class LanguageInfo
{
    /// <summary>
    /// 取得或设置语言ID。
    /// </summary>
    [Form, Required]
    [Column(IsViewLink = true, Width = 120)]
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置语言名称。
    /// </summary>
    [Form, Required]
    [DisplayName("名称")]
    [Column(IsQuery = true, Width = 120)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置语言图标。
    /// </summary>
    [Required]
    [Column, Form]
    [DisplayName("图标")]
    public string Icon { get; set; }
}