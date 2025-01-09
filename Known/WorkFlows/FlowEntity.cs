namespace Known.WorkFlows;

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
    /// 提交流程表单时，验证流程实体虚方法。
    /// </summary>
    /// <param name="context">上下文对象。</param>
    /// <returns>验证结果。</returns>
    public virtual Result ValidCommit(Context context) => base.Validate(context);
}