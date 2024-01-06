namespace Known.WorkFlows;

public enum FlowPageType { None, Apply, Verify, Query }

public class FlowStatus
{
    private FlowStatus() { }

    public const string Open = "Open";
    public const string Over = "Over";
    public const string Stop = "Stop";

    internal const string StepCreate = "Create";
    internal const string StepSubmit = "Submit";
    internal const string StepRevoke = "Revoke";
    internal const string StepVerify = "Verify";
    internal const string StepAssign = "Assign";
    internal const string StepStopped = "Stopped";
    internal const string StepRestart = "Restart";
    internal const string StepEnd = "End";

    public const string Save = "Save";
    public const string Revoked = "Revoked";
    public const string Verifing = "Verifing";
    public const string VerifyPass = "Pass";
    public const string VerifyFail = "Fail";
    public const string Reapply = "Reapply";
}

public class FlowEntity : EntityBase
{
    public virtual string BizStatus { get; set; }
    public virtual string CurrStep { get; set; }
    public virtual string CurrBy { get; set; }
    public virtual string ApplyBy { get; set; }
    public virtual DateTime? ApplyTime { get; set; }
    public virtual string VerifyBy { get; set; }
    public virtual DateTime? VerifyTime { get; set; }
    public virtual string VerifyNote { get; set; }

    public virtual bool CanSubmit => BizStatus == FlowStatus.Save || BizStatus == FlowStatus.Revoked || BizStatus == FlowStatus.VerifyFail || BizStatus == FlowStatus.Reapply;
    public virtual bool CanRevoke => BizStatus == FlowStatus.Verifing;
    public virtual Result ValidCommit(Context context) => base.Validate(context);
}