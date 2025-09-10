namespace Known.Entities;

/// <summary>
/// 工作流实体类。
/// </summary>
[DisplayName("工作流")]
public class SysFlow : EntityBase
{
    /// <summary>
    /// 取得或设置流程代码。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("流程代码")]
    public string FlowCode { get; set; }

    /// <summary>
    /// 取得或设置流程名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("流程名称")]
    public string FlowName { get; set; }

    /// <summary>
    /// 取得或设置流程状态（开启，结束，终止）。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("流程状态")]
    public string FlowStatus { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("业务ID")]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置业务描述。
    /// </summary>
    [Required]
    [MaxLength(200)]
    [DisplayName("业务描述")]
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置业务Url。
    /// </summary>
    [Required]
    [MaxLength(200)]
    [DisplayName("业务Url")]
    public string BizUrl { get; set; }

    /// <summary>
    /// 取得或设置业务状态。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("业务状态")]
    public string BizStatus { get; set; }

    /// <summary>
    /// 取得或设置当前步骤。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("当前步骤")]
    public string CurrStep { get; set; }

    /// <summary>
    /// 取得或设置当前执行人。
    /// </summary>
    [Required]
    [MaxLength(200)]
    [DisplayName("当前执行人")]
    public string CurrBy { get; set; }

    /// <summary>
    /// 取得或设置上一步骤。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("上一步骤")]
    public string PrevStep { get; set; }

    /// <summary>
    /// 取得或设置上一步执行人。
    /// </summary>
    [MaxLength(200)]
    [DisplayName("上一步执行人")]
    public string PrevBy { get; set; }

    /// <summary>
    /// 取得或设置下一步骤。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("下一步骤")]
    public string NextStep { get; set; }

    /// <summary>
    /// 取得或设置下一步执行人。
    /// </summary>
    [MaxLength(200)]
    [DisplayName("下一步执行人")]
    public string NextBy { get; set; }

    /// <summary>
    /// 取得或设置申请人。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("申请人")]
    public string ApplyBy { get; set; }

    /// <summary>
    /// 取得或设置申请时间。
    /// </summary>
    [DisplayName("申请时间")]
    public DateTime? ApplyTime { get; set; }

    /// <summary>
    /// 取得或设置审核人。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("审核人")]
    public string VerifyBy { get; set; }

    /// <summary>
    /// 取得或设置审核时间。
    /// </summary>
    [DisplayName("审核时间")]
    public DateTime? VerifyTime { get; set; }

    /// <summary>
    /// 取得或设置审核意见。
    /// </summary>
    [DisplayName("审核意见")]
    public string VerifyNote { get; set; }
}