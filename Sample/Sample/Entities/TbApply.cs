namespace Sample.Entities;

/// <summary>
/// 申请单实体类。
/// </summary>
public class TbApply : EntityBase
{
    /// <summary>
    /// 取得或设置业务类型。
    /// </summary>
    [Column("业务类型", "", true, "1", "50")]
    public ApplyType BizType { get; set; }

    /// <summary>
    /// 取得或设置业务单号。
    /// </summary>
    [Column("业务单号", "", true, "1", "50")]
    public string? BizNo { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    [Column("业务名称", "", true, "1", "100")]
    public string? BizTitle { get; set; }

    /// <summary>
    /// 取得或设置业务内容。
    /// </summary>
    [Column("业务内容", "", false)]
    public string? BizContent { get; set; }

    /// <summary>
    /// 取得或设置业务附件。
    /// </summary>
    [Column("业务附件", "", false, "1", "250")]
    public string? BizFile { get; set; }

    [Column("流程状态")] public virtual string? BizStatus { get; set; }
    [Column("当前人")] public virtual string? CurrBy { get; set; }
    [Column("申请人")] public virtual string? ApplyBy { get; set; }
    [Column("申请时间")] public virtual DateTime? ApplyTime { get; set; }
    [Column("审核人")] public virtual string? VerifyBy { get; set; }
    [Column("审核时间")] public virtual DateTime? VerifyTime { get; set; }
    [Column("审核意见")] public virtual string? VerifyNote { get; set; }

    public virtual bool CanSubmit => BizStatus == FlowStatus.Save || BizStatus == FlowStatus.Revoked || BizStatus == FlowStatus.VerifyFail || BizStatus == FlowStatus.ReApply;
    public virtual bool CanRevoke => BizStatus == FlowStatus.Verifing;

    public Result ValidCommit()
    {
        var vr = base.Validate();
        vr.Required("业务内容", BizContent);
        return vr;
    }
}