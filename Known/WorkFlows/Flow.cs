namespace Known.WorkFlows;

public enum FlowPageType { None, Apply, Verify, Query }

public class FlowStatus
{
    private FlowStatus() { }
    //TODO:流程数据语言切换
    public const string Open = "开启";
    public const string Over = "结束";
    public const string Stop = "终止";

    internal const string StepCreate = "创建流程";
    internal const string StepSubmit = "提交流程";
    internal const string StepRevoke = "撤回流程";
    internal const string StepVerify = "审核流程";
    internal const string StepAssign = "任务分配";
    internal const string StepStop = "终止流程";
    internal const string StepRepeat = "重启流程";
    internal const string StepOver = "结束流程";

    public const string Save = "暂存";
    public const string Revoked = "已撤回";
    public const string Verifing = "待审核";
    public const string VerifyPass = "审核通过";
    public const string VerifyFail = "审核退回";
    public const string ReApply = "重新申请";
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

    public virtual bool CanSubmit => BizStatus == FlowStatus.Save || BizStatus == FlowStatus.Revoked || BizStatus == FlowStatus.VerifyFail || BizStatus == FlowStatus.ReApply;
    public virtual bool CanRevoke => BizStatus == FlowStatus.Verifing;
    public virtual Result ValidCommit(Context context) => base.Validate(context);
}