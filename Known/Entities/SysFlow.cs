namespace Known.Entities;

/// <summary>
/// 工作流实体类。
/// </summary>
public class SysFlow : EntityBase
{
    /// <summary>
    /// 取得或设置流程代码。
    /// </summary>
    [Column("流程代码", "", true, "1", "50")]
    public string FlowCode { get; set; }

    /// <summary>
    /// 取得或设置流程名称。
    /// </summary>
    [Column("流程名称", "", true, "1", "50")]
    public string FlowName { get; set; }

    /// <summary>
    /// 取得或设置流程状态（开启，结束，终止）。
    /// </summary>
    [Column("流程状态", "", true, "1", "50")]
    public string FlowStatus { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [Column("业务ID", "", true, "1", "50", IsGrid = false)]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置业务描述。
    /// </summary>
    [Column("业务描述", "", true, "1", "200")]
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置业务Url。
    /// </summary>
    [Column("业务Url", "", true, "1", "200")]
    public string BizUrl { get; set; }

    /// <summary>
    /// 取得或设置业务状态。
    /// </summary>
    [Column("业务状态", "", true, "1", "50")]
    public string BizStatus { get; set; }

    /// <summary>
    /// 取得或设置当前步骤。
    /// </summary>
    [Column("当前步骤", "", true, "1", "50")]
    public string CurrStep { get; set; }

    /// <summary>
    /// 取得或设置当前执行人。
    /// </summary>
    [Column("当前执行人", "", true, "1", "200")]
    public string CurrBy { get; set; }

    /// <summary>
    /// 取得或设置上一步骤。
    /// </summary>
    [Column("上一步骤", "", false, "1", "50")]
    public string PrevStep { get; set; }

    /// <summary>
    /// 取得或设置上一步执行人。
    /// </summary>
    [Column("上一步执行人", "", false, "1", "200")]
    public string PrevBy { get; set; }

    /// <summary>
    /// 取得或设置下一步骤。
    /// </summary>
    [Column("下一步骤", "", false, "1", "50")]
    public string NextStep { get; set; }

    /// <summary>
    /// 取得或设置下一步执行人。
    /// </summary>
    [Column("下一步执行人", "", false, "1", "200")]
    public string NextBy { get; set; }

    /// <summary>
    /// 取得或设置申请人。
    /// </summary>
    [Column("申请人", "", false, "1", "50")]
    public string ApplyBy { get; set; }

    /// <summary>
    /// 取得或设置申请时间。
    /// </summary>
    [Column("申请时间", "", false)]
    public DateTime? ApplyTime { get; set; }

    /// <summary>
    /// 取得或设置审核人。
    /// </summary>
    [Column("审核人", "", false, "1", "50")]
    public string VerifyBy { get; set; }

    /// <summary>
    /// 取得或设置审核时间。
    /// </summary>
    [Column("审核时间", "", false)]
    public DateTime? VerifyTime { get; set; }

    /// <summary>
    /// 取得或设置审核意见。
    /// </summary>
    [Column("审核人意见", "", false, "1", "500")]
    public string VerifyNote { get; set; }
}