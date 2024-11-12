namespace Known.WorkFlow;

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