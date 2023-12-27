using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Entities;

/// <summary>
/// 系统任务实体类。
/// </summary>
public class SysTask : EntityBase
{
    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [DisplayName("业务ID")]
    [Required]
    [MaxLength(50)]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    [DisplayName("类型")]
    [Required]
    [MaxLength(50)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [DisplayName("名称")]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置执行目标。
    /// </summary>
    [DisplayName("执行目标")]
    [MaxLength(200)]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置执行状态。
    /// </summary>
    [Category(nameof(TaskStatus))]
    [DisplayName("执行状态")]
    [Required]
    [MaxLength(50)]
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置开始时间。
    /// </summary>
    [DisplayName("开始时间")]
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 取得或设置结束时间。
    /// </summary>
    [DisplayName("结束时间")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [DisplayName("备注")]
    public string Note { get; set; }
}