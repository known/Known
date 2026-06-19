namespace Known.Reports;

/// <summary>
/// 系统报表实体类。
/// </summary>
[DisplayName("系统报表")]
public class SysReport : EntityBase
{
    /// <summary>
    /// 取得或设置子系统ID。
    /// </summary>
    [MaxLength(50)]
    [Column]
    [DisplayName("子系统ID")]
    public string SysId { get; set; }

    /// <summary>
    /// 取得或设置是否是系统固定的，不能删除的报表。
    /// </summary>
    [Column]
    [DisplayName("是否固定")]
    public bool IsFixed { get; set; }

    /// <summary>
    /// 取得或设置报表名称。
    /// </summary>
    [Required]
    [MaxLength(250)]
    [DisplayName("报表名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置栅格行数。
    /// </summary>
    public int? GridRow { get; set; } = 1;

    /// <summary>
    /// 取得或设置栅格列数。
    /// </summary>
    public int? GridColumn { get; set; } = 1;

    /// <summary>
    /// 取得或设置备注信息。
    /// </summary>
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置报表块列表。
    /// </summary>
    public List<ReportBlock> Blocks { get; set; } = [];
}