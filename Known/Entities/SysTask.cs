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
    [Column]
    [DisplayName("业务ID")]
    [Required(ErrorMessage = "业务ID不能为空！")]
    [MaxLength(50)]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    [Column, Grid, Query]
    [DisplayName("类型")]
    [Required(ErrorMessage = "类型不能为空！")]
    [MaxLength(50)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Column, Grid, Query]
    [DisplayName("名称")]
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置执行目标。
    /// </summary>
    [Column, Grid]
    [DisplayName("执行目标")]
    [MaxLength(200)]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置执行状态。
    /// </summary>
    [Column, Grid]
    [Code(Category = nameof(TaskStatus))]
    [DisplayName("执行状态")]
    [Required(ErrorMessage = "执行状态不能为空！")]
    [MaxLength(50)]
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置开始时间。
    /// </summary>
    [Column, Grid]
    [DisplayName("开始时间")]
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 取得或设置结束时间。
    /// </summary>
    [Column, Grid]
    [DisplayName("结束时间")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column, Grid]
    [DisplayName("备注")]
    public string Note { get; set; }
}