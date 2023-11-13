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
    [MinLength(1), MaxLength(50)]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true)]
    [DisplayName("类型")]
    [Required(ErrorMessage = "类型不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true)]
    [DisplayName("名称")]
    [Required(ErrorMessage = "名称不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置执行目标。
    /// </summary>
    [Column(IsGrid = true)]
    [DisplayName("执行目标")]
    [MinLength(1), MaxLength(200)]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置执行状态。
    /// </summary>
    [Column(IsGrid = true, CodeType = nameof(TaskStatus))]
    [DisplayName("执行状态")]
    [Required(ErrorMessage = "执行状态不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置开始时间。
    /// </summary>
    [Column(IsGrid = true)]
    [DisplayName("开始时间")]
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 取得或设置结束时间。
    /// </summary>
    [Column(IsGrid = true)]
    [DisplayName("结束时间")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column(IsGrid = true)]
    [DisplayName("备注")]
    public string Note { get; set; }
}