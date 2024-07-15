namespace Sample.Entities;

/// <summary>
/// 申请单表体实体类。
/// </summary>
public class TbApplyList : EntityBase
{
    /// <summary>
    /// 取得或设置表头ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string HeadId { get; set; }

    /// <summary>
    /// 取得或设置项目。
    /// </summary>
    [Required]
    [MaxLength(100)]
    [DisplayName("项目")]
    public string Item { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [DisplayName("备注")]
    public string Note { get; set; }
}