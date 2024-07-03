namespace Sample.Entities;

/// <summary>
/// 申请单实体类。
/// </summary>
public class TbApply : FlowEntity
{
    /// <summary>
    /// 取得或设置业务类型。
    /// </summary>
    [Category(nameof(ApplyType))]
    [Required]
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置业务单号。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string BizNo { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string BizTitle { get; set; }

    /// <summary>
    /// 取得或设置业务内容。
    /// </summary>
    [MaxLength(4000)]
    public string BizContent { get; set; }

    /// <summary>
    /// 取得或设置业务附件。
    /// </summary>
    [MaxLength(250)]
    public string BizFile { get; set; }

    public override Result ValidCommit(Context context)
    {
        var vr = base.Validate(context);
        vr.Required(context, context.Language["BizContent"], BizContent);
        return vr;
    }
}