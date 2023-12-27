using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Entities;

/// <summary>
/// 数据字典实体类。
/// </summary>
public class SysDictionary : EntityBase
{
    public SysDictionary()
    {
        Enabled = true;
    }

    /// <summary>
    /// 取得或设置类别。
    /// </summary>
    [Category(Constants.DicCategory)]
    [DisplayName("类别")]
    [Required]
    [MaxLength(50)]
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置类别名称。
    /// </summary>
    [DisplayName("类别名称")]
    [MaxLength(50)]
    public string CategoryName { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [DisplayName("代码")]
    [Required]
    [MaxLength(100)]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [DisplayName("名称")]
    [MaxLength(150)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [DisplayName("顺序")]
    [Required]
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [DisplayName("状态")]
    [Required]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [DisplayName("备注")]
    [MaxLength(500)]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置子字典。
    /// </summary>
    public string Child { get; set; }
}