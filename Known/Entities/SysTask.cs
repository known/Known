﻿namespace Known.Entities;

/// <summary>
/// 系统任务实体类。
/// </summary>
public class SysTask : EntityBase
{
    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置执行目标。
    /// </summary>
    [Column]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置执行状态。
    /// </summary>
    [Category(nameof(TaskJobStatus))]
    [Required]
    [MaxLength(50)]
    [Column]
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置开始时间。
    /// </summary>
    [Column]
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 取得或设置结束时间。
    /// </summary>
    [Column]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    public string Note { get; set; }
}