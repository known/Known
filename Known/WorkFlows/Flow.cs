using System.ComponentModel;

namespace Known.WorkFlows;

public class FlowStatus
{
    private FlowStatus() { }

    public const string Open = "开启";
    public const string Over = "结束";
    public const string Stop = "终止";

    public const string Save = "暂存";
    public const string Revoked = "已撤回";
    public const string Verifing = "待审核";
    public const string VerifyPass = "审核通过";
    public const string VerifyFail = "审核退回";
    public const string ReApply = "重新申请";
}

public class FlowEntity : EntityBase
{
    [Column(IsGrid = true, IsQuery = true)]
    [DisplayName("流程状态")]
    public virtual string BizStatus { get; set; }
    
    [Column]
    [DisplayName("当前人")]
    public virtual string CurrBy { get; set; }
    
    [Column(IsGrid = true)]
    [DisplayName("申请人")]
    public virtual string ApplyBy { get; set; }
    
    [Column(IsGrid = true)]
    [DisplayName("申请时间")]
    public virtual DateTime? ApplyTime { get; set; }
    
    [Column(IsGrid = true)]
    [DisplayName("审核人")]
    public virtual string VerifyBy { get; set; }
    
    [Column(IsGrid = true)]
    [DisplayName("审核时间")]
    public virtual DateTime? VerifyTime { get; set; }
    
    [Column(IsGrid = true)]
    [DisplayName("审核意见")]
    public virtual string VerifyNote { get; set; }

    public virtual bool CanSubmit => BizStatus == FlowStatus.Save || BizStatus == FlowStatus.Revoked || BizStatus == FlowStatus.VerifyFail || BizStatus == FlowStatus.ReApply;
    public virtual bool CanRevoke => BizStatus == FlowStatus.Verifing;
    public virtual Result ValidCommit() => base.Validate();
}