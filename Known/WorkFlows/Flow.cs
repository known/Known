namespace Known.WorkFlows;

/// <summary>
/// 流程页面类型枚举。
/// </summary>
public enum FlowPageType
{
    /// <summary>
    /// 非流程页面。
    /// </summary>
    None,
    /// <summary>
    /// 申请页面。
    /// </summary>
    Apply,
    /// <summary>
    /// 审核页面。
    /// </summary>
    Verify,
    /// <summary>
    /// 查询页面。
    /// </summary>
    Query
}

/// <summary>
/// 流程状态类。
/// </summary>
public class FlowStatus
{
    private FlowStatus() { }

    /// <summary>
    /// 流程已启动。
    /// </summary>
    public const string Open = "Open";
    /// <summary>
    /// 流程已结束。
    /// </summary>
    public const string Over = "Over";
    /// <summary>
    /// 流程已停止。
    /// </summary>
    public const string Stop = "Stop";

    internal const string StepCreate = "Create";
    internal const string StepSubmit = "Submit";
    internal const string StepRevoke = "Revoke";
    internal const string StepVerify = "Verify";
    internal const string StepAssign = "Assign";
    internal const string StepStopped = "Stopped";
    internal const string StepRestart = "Restart";
    internal const string StepEnd = "End";

    /// <summary>
    /// 暂存。
    /// </summary>
    public const string Save = "Save";
    /// <summary>
    /// 已撤回。
    /// </summary>
    public const string Revoked = "Revoked";
    /// <summary>
    /// 待审核。
    /// </summary>
    public const string Verifing = "Verifing";
    /// <summary>
    /// 审核通过。
    /// </summary>
    public const string VerifyPass = "Pass";
    /// <summary>
    /// 审核退回。
    /// </summary>
    public const string VerifyFail = "Fail";
    /// <summary>
    /// 重新申请。
    /// </summary>
    public const string Reapply = "Reapply";
}

/// <summary>
/// 流程实体基类。
/// </summary>
public class FlowEntity : EntityBase
{
    /// <summary>
    /// 取得或设置流程状态。
    /// </summary>
    public virtual string BizStatus { get; set; }

    /// <summary>
    /// 取得或设置当前步骤。
    /// </summary>
    public virtual string CurrStep { get; set; }

    /// <summary>
    /// 取得或设置当前操作人。
    /// </summary>
    public virtual string CurrBy { get; set; }

    /// <summary>
    /// 取得或设置申请人。
    /// </summary>
    public virtual string ApplyBy { get; set; }

    /// <summary>
    /// 取得或设置申请时间。
    /// </summary>
    public virtual DateTime? ApplyTime { get; set; }

    /// <summary>
    /// 取得或设置审核人。
    /// </summary>
    public virtual string VerifyBy { get; set; }

    /// <summary>
    /// 取得或设置审核时间。
    /// </summary>
    public virtual DateTime? VerifyTime { get; set; }

    /// <summary>
    /// 取得或设置审核备注信息。
    /// </summary>
    public virtual string VerifyNote { get; set; }

    /// <summary>
    /// 取得流程是否可以提交。
    /// </summary>
    public virtual bool CanSubmit => BizStatus == FlowStatus.Save || BizStatus == FlowStatus.Revoked || BizStatus == FlowStatus.VerifyFail || BizStatus == FlowStatus.Reapply;
    
    /// <summary>
    /// 取得流程是否可以撤回。
    /// </summary>
    public virtual bool CanRevoke => BizStatus == FlowStatus.Verifing;

    /// <summary>
    /// 提交流程表单时，验证流程实体虚方法。
    /// </summary>
    /// <param name="context">上下文对象。</param>
    /// <returns>验证结果。</returns>
    public virtual Result ValidCommit(Context context) => base.Validate(context);
}