using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.WorkFlows;

/// <summary>
/// 工作流日志实体类。
/// </summary>
public class SysFlowLog : EntityBase
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
    /// 取得或设置步骤。
    /// </summary>
    [Column, Grid]
    [DisplayName("步骤")]
    [Required(ErrorMessage = "步骤不能为空！")]
    [MaxLength(50)]
    public string StepName { get; set; }

    /// <summary>
    /// 取得或设置操作人。
    /// </summary>
    [Column, Grid]
    [DisplayName("操作人")]
    [Required(ErrorMessage = "操作人不能为空！")]
    [MaxLength(50)]
    public string ExecuteBy { get; set; }

    /// <summary>
    /// 取得或设置操作时间。
    /// </summary>
    [Column, Grid]
    [DisplayName("操作时间")]
    [Required(ErrorMessage = "操作时间不能为空！")]
    public DateTime ExecuteTime { get; set; }

    /// <summary>
    /// 取得或设置操作结果（通过、退回、撤回）。
    /// </summary>
    [Column, Grid]
    [DisplayName("操作结果")]
    [Required(ErrorMessage = "操作结果不能为空！")]
    [MaxLength(50)]
    public string Result { get; set; }

    /// <summary>
    /// 取得或设置操作内容。
    /// </summary>
    [Column, Grid]
    [DisplayName("操作内容")]
    [MaxLength(1000)]
    public string Note { get; set; }
}