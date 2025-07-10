namespace Known;

/// <summary>
/// 字典类型枚举。
/// </summary>
public enum DictionaryType
{
    /// <summary>
    /// 默认。
    /// </summary>
    None,
    /// <summary>
    /// 包含子字典。
    /// </summary>
    Child,
    /// <summary>
    /// 包含扩展文本。
    /// </summary>
    Text,
    /// <summary>
    /// 包含扩展图片。
    /// </summary>
    Image
}

/// <summary>
/// 数据字典信息类。
/// </summary>
[DisplayName("数据字典")]
public class DictionaryInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public DictionaryInfo()
    {
        Id = Utils.GetNextId();
        Enabled = true;
    }

    /// <summary>
    /// 取得或设置实体ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置扩展数据。
    /// </summary>
    public string Extension { get; set; }

    /// <summary>
    /// 取得或设置类别。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(Width = 120)]
    [DisplayName("类别")]
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置类别名称。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("类别名称")]
    public string CategoryName { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column(Width = 120, IsQuery = true, IsViewLink = true)]
    [Form]
    [DisplayName("代码")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [MaxLength(150)]
    [Column(Width = 120, IsQuery = true)]
    [Form]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [Required]
    [Column(Width = 80)]
    [Form]
    [DisplayName("顺序")]
    public int? Sort { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    [Column(Width = 80)]
    [Form(Type = nameof(FieldType.Switch))]
    [DisplayName("状态")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [Form(Type = nameof(FieldType.TextArea))]
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置子字典。
    /// </summary>
    [DisplayName("子字典")]
    public string Child { get; set; }

    /// <summary>
    /// 取得或设置字典类型。
    /// </summary>
    public DictionaryType DicType { get; set; }
}