namespace Known.Entities;

/// <summary>
/// 后台任务实体类。
/// </summary>
[DisplayName("后台任务")]
public class SysTask : EntityBase
{
    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("业务ID")]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(Width = 120, IsQuery = true)]
    [DisplayName("类型")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置名称 
    [Required]
    [MaxLength(50)]
    [Column(Width = 150, IsQuery = true)]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置执行目标。
    /// </summary>
    [Column(Width = 250)]
    [DisplayName("目标")]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置执行状态。
    /// </summary>
    [Category(nameof(TaskJobStatus))]
    [Required]
    [MaxLength(50)]
    [Column(Width = 100)]
    [DisplayName("状态")]
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置开始时间。
    /// </summary>
    [Column(Width = 140)]
    [DisplayName("开始时间")]
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 取得或设置结束时间。
    /// </summary>
    [Column(Width = 140)]
    [DisplayName("结束时间")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column(Width = 200)]
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置任务关联的附件信息。
    /// </summary>
    public virtual AttachInfo File { get; set; }
}