namespace Sample.Entities;

/// <summary>
/// 申请单实体类。
/// </summary>
public class TbApply : FlowEntity
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

    public override Result ValidCommit()
    {
        var vr = base.Validate();
        vr.Required("业务内容", BizContent);
        return vr;
    }
}