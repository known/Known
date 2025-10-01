namespace Known.Entities;

/// <summary>
/// 数据字典实体类。
/// </summary>
[DisplayName("数据字典")]
public class SysDictionary : EntityBase
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public SysDictionary()
    {
        Enabled = true;
    }

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
    [Column(Width = 150, IsQuery = true)]
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
    public virtual DictionaryType DicType { get; set; }
}