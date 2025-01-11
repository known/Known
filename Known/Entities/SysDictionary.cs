namespace Known.Entities;

/// <summary>
/// 数据字典实体类。
/// </summary>
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
    [Column]
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置类别名称。
    /// </summary>
    [MaxLength(50)]
    public string CategoryName { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column(IsQuery = true)]
    [Form]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [MaxLength(150)]
    [Column(IsQuery = true)]
    [Form]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [Required]
    [Column]
    [Form]
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    [Column]
    [Form]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [Form]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置子字典。
    /// </summary>
    public string Child { get; set; }
}