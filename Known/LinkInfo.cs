namespace Known;

/// <summary>
/// 连接信息类。
/// </summary>
public class LinkInfo
{
    /// <summary>
    /// 取得或设置连接标题。
    /// </summary>
    [Required]
    [DisplayName("标题")]
    [Form]
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置连接图标。
    /// </summary>
    [Required]
    [DisplayName("图标")]
    [Form(Type = nameof(FieldType.Custom), CustomField = nameof(IconPicker))]
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置连接URL。
    /// </summary>
    [Required]
    [DisplayName("URL")]
    [Form]
    public string Url { get; set; }

    /// <summary>
    /// 取得或设连接打开目标位置。
    /// </summary>
    [Required]
    [DisplayName("目标")]
    [Form(Type = nameof(FieldType.RadioList))]
    [Category(nameof(LinkTarget))]
    public string Target { get; set; }
}

/// <summary>
/// 连接打开目标位置。
/// </summary>
public enum LinkTarget
{
    /// <summary>
    /// 当前窗口。
    /// </summary>
    None,
    /// <summary>
    /// 新窗口。
    /// </summary>
    Blank,
    /// <summary>
    /// iFrame窗口。
    /// </summary>
    IFrame
}