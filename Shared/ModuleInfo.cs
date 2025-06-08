namespace Known;

/// <summary>
/// 框架模块信息类。
/// </summary>
public partial class ModuleInfo
{
    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; } = Utils.GetNextId();

    /// <summary>
    /// 取得或设置上级。
    /// </summary>
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置图标。
    /// </summary>
    [Required]
    [DisplayName("图标")]
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置类型（Menu/Page/Link）。
    /// </summary>
    [DisplayName("类型")]
    public string Type { get; set; } = nameof(MenuType.Menu);

    /// <summary>
    /// 取得或设置目标（None/Blank/IFrame）。
    /// </summary>
    [Required]
    [DisplayName("目标")]
    public string Target { get; set; } = nameof(LinkTarget.None);

    /// <summary>
    /// 取得或设置Url地址。
    /// </summary>
    [DisplayName("地址")]
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [Required]
    [DisplayName("顺序")]
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置可用。
    /// </summary>
    [Required]
    [DisplayName("可用")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 取得或设置是否是代码生成的模块。
    /// </summary>
    public bool IsCode { get; set; }

    /// <summary>
    /// 取得或设置是否移动模块。
    /// </summary>
    [JsonIgnore]
    public bool IsMoveUp { get; set; }

    internal string ParentName { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{Name}({Url})";
}