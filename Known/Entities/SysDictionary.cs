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
    [Column(IsGrid = true, IsQuery = true, CodeType = Constants.DicCategory, IsQueryAll = false)]
    [DisplayName("类别")]
    [Required(ErrorMessage = "类别不能为空！")]
    [MaxLength(50)]
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置类别名称。
    /// </summary>
    [Column]
    [DisplayName("类别名称")]
    [MaxLength(50)]
    public string CategoryName { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true, IsForm = true, IsViewLink = true)]
    [DisplayName("代码")]
    [Required(ErrorMessage = "代码不能为空！")]
    [MaxLength(100)]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true, IsForm = true)]
    [DisplayName("名称")]
    [MaxLength(150)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("顺序")]
    [Required(ErrorMessage = "顺序不能为空！")]
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("状态")]
    [Required(ErrorMessage = "状态不能为空！")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("备注")]
    [MaxLength(500)]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置子字典。
    /// </summary>
    [Column]
    [DisplayName("子字典")]
    public string Child { get; set; }

    public virtual bool HasChild { get; set; }
    public virtual List<CodeName> Children => Utils.FromJson<List<CodeName>>(Child);
}