namespace Known.Entities;

/// <summary>
/// 通用同步日志实体类。
/// </summary>
[DisplayName("同步日志")]
public class SysSyncLog : EntityBase
{
    /// <summary>
    /// 取得或设置业务类型。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(Width = 120, IsQuery = true)]
    [DisplayName("业务类型")]
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置期间。
    /// </summary>
    [MaxLength(20)]
    [Column(Width = 100)]
    [DisplayName("期间")]
    public string Period { get; set; }

    /// <summary>
    /// 取得或设置业务单号。
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column(Width = 120, IsQuery = true)]
    [DisplayName("业务单号")]
    public string BizNo { get; set; }

    /// <summary>
    /// 取得或设置业务子号。
    /// </summary>
    [MaxLength(100)]
    [Column(Width = 120)]
    [DisplayName("业务子号")]
    public string BizSubNo { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    [MaxLength(100)]
    [Column(Width = 120)]
    [DisplayName("业务名称")]
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置同步类型。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(Width = 120)]
    [DisplayName("同步类型")]
    public string SyncType { get; set; }

    /// <summary>
    /// 取得或设置重试次数。
    /// </summary>
    [Column(Width = 120)]
    [DisplayName("重试次数")]
    public int RetryCount { get; set; }

    /// <summary>
    /// 取得或设置同步结果。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(Width = 120)]
    [DisplayName("同步结果")]
    public string SyncResult { get; set; }

    /// <summary>
    /// 取得或设置回执号。
    /// </summary>
    [MaxLength(200)]
    [Column(Width = 180)]
    [DisplayName("回执号")]
    public string ReceiptNo { get; set; }

    /// <summary>
    /// 取得或设置回执消息。
    /// </summary>
    [Column(Width = 280)]
    [DisplayName("回执消息")]
    public string ReceiptMessage { get; set; }

    /// <summary>
    /// 取得或设置同步时间。
    /// </summary>
    [Column(Width = 160, Type = FieldType.DateTime)]
    [DisplayName("同步时间")]
    public DateTime? SyncTime { get; set; }
}