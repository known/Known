namespace Known.WorkFlows;

/// <summary>
/// 工作流扩展类。
/// </summary>
public static partial class FlowExtension
{
    /// <summary>
    /// 判断流程是否可以提交。
    /// </summary>
    /// <param name="entity">流程实体对象。</param>
    /// <returns></returns>
    public static bool CanSubmit(this FlowEntity entity)
    {
        return entity.BizStatus == FlowStatus.Save ||
               entity.BizStatus == FlowStatus.Revoked ||
               entity.BizStatus == FlowStatus.VerifyFail ||
               entity.BizStatus == FlowStatus.Reapply;
    }

    /// <summary>
    /// 判断流程是否可以撤回。
    /// </summary>
    /// <param name="entity">流程实体对象。</param>
    /// <returns></returns>
    public static bool CanRevoke(this FlowEntity entity)
    {
        return entity.BizStatus == FlowStatus.Verifing;
    }
}