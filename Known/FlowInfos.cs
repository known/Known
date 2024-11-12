namespace Known;

/// <summary>
/// 工作流配置信息类。
/// </summary>
public class FlowInfo
{
    /// <summary>
    /// 取得或设置工作流ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置工作流名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置工作流描述。
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置工作流表单对话框宽度。
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// 取得或设置工作流表单对话框高度。
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// 取得或设置工作流当前步骤。
    /// </summary>
    public int Current { get; set; }

    /// <summary>
    /// 取得或设置工作流步骤列表。
    /// </summary>
    public List<FlowStepInfo> Steps { get; set; } = [];
}

/// <summary>
/// 工作流步骤配置信息类。
/// </summary>
public class FlowStepInfo
{
    /// <summary>
    /// 取得或设置步骤ID。
    /// </summary>
    [DisplayName("Id")]
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置步骤名称。
    /// </summary>
    [DisplayName("Name")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置步骤操作用户。
    /// </summary>
    [DisplayName("User")]
    public string User { get; set; }

    /// <summary>
    /// 取得或设置步骤操作角色。
    /// </summary>
    [DisplayName("Role")]
    public string Role { get; set; }

    /// <summary>
    /// 取得或设置步骤审核通过的流程状态。
    /// </summary>
    [DisplayName("Pass")]
    public string Pass { get; set; }

    /// <summary>
    /// 取得或设置步骤审核退回的流程状态。
    /// </summary>
    [DisplayName("Fail")]
    public string Fail { get; set; }
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
    /// 提交流程表单时，验证流程实体虚方法。
    /// </summary>
    /// <param name="context">上下文对象。</param>
    /// <returns>验证结果。</returns>
    public virtual Result ValidCommit(Context context) => base.Validate(context);
}