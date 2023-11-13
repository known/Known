using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.WorkFlows;

/// <summary>
/// 工作流实体类。
/// </summary>
public class SysFlow : EntityBase
{
    /// <summary>
    /// 取得或设置流程代码。
    /// </summary>
    [Column]
    [DisplayName("流程代码")]
    [Required(ErrorMessage = "流程代码不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string FlowCode { get; set; }

    /// <summary>
    /// 取得或设置流程名称。
    /// </summary>
    [Column]
    [DisplayName("流程名称")]
    [Required(ErrorMessage = "流程名称不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string FlowName { get; set; }

    /// <summary>
    /// 取得或设置流程状态（开启，结束，终止）。
    /// </summary>
    [Column]
    [DisplayName("流程状态")]
    [Required(ErrorMessage = "流程状态不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string FlowStatus { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [Column]
    [DisplayName("业务ID")]
    [Required(ErrorMessage = "业务ID不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置业务描述。
    /// </summary>
    [Column]
    [DisplayName("业务描述")]
    [Required(ErrorMessage = "业务描述不能为空！")]
    [MinLength(1), MaxLength(200)]
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置业务Url。
    /// </summary>
    [Column]
    [DisplayName("业务Url")]
    [Required(ErrorMessage = "业务Url不能为空！")]
    [MinLength(1), MaxLength(200)]
    public string BizUrl { get; set; }

    /// <summary>
    /// 取得或设置业务状态。
    /// </summary>
    [Column]
    [DisplayName("业务状态")]
    [Required(ErrorMessage = "业务状态不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string BizStatus { get; set; }

    /// <summary>
    /// 取得或设置当前步骤。
    /// </summary>
    [Column]
    [DisplayName("当前步骤")]
    [Required(ErrorMessage = "当前步骤不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string CurrStep { get; set; }

    /// <summary>
    /// 取得或设置当前执行人。
    /// </summary>
    [Column]
    [DisplayName("当前执行人")]
    [Required(ErrorMessage = "当前执行人不能为空！")]
    [MinLength(1), MaxLength(200)]
    public string CurrBy { get; set; }

    /// <summary>
    /// 取得或设置上一步骤。
    /// </summary>
    [Column]
    [DisplayName("上一步骤")]
    [MinLength(1), MaxLength(50)]
    public string PrevStep { get; set; }

    /// <summary>
    /// 取得或设置上一步执行人。
    /// </summary>
    [Column]
    [DisplayName("上一步执行人")]
    [MinLength(1), MaxLength(200)]
    public string PrevBy { get; set; }

    /// <summary>
    /// 取得或设置下一步骤。
    /// </summary>
    [Column]
    [DisplayName("下一步骤")]
    [MinLength(1), MaxLength(50)]
    public string NextStep { get; set; }

    /// <summary>
    /// 取得或设置下一步执行人。
    /// </summary>
    [Column]
    [DisplayName("下一步执行人")]
    [MinLength(1), MaxLength(200)]
    public string NextBy { get; set; }

    /// <summary>
    /// 取得或设置申请人。
    /// </summary>
    [Column]
    [DisplayName("申请人")]
    [MinLength(1), MaxLength(50)]
    public string ApplyBy { get; set; }

    /// <summary>
    /// 取得或设置申请时间。
    /// </summary>
    [Column]
    [DisplayName("申请时间")]
    public DateTime? ApplyTime { get; set; }

    /// <summary>
    /// 取得或设置审核人。
    /// </summary>
    [Column]
    [DisplayName("审核人")]
    [MinLength(1), MaxLength(50)]
    public string VerifyBy { get; set; }

    /// <summary>
    /// 取得或设置审核时间。
    /// </summary>
    [Column]
    [DisplayName("审核时间")]
    public DateTime? VerifyTime { get; set; }

    /// <summary>
    /// 取得或设置审核意见。
    /// </summary>
    [Column]
    [DisplayName("审核人意见")]
    [MinLength(1), MaxLength(500)]
    public string VerifyNote { get; set; }
}