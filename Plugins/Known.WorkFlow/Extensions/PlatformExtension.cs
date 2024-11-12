namespace Known.WorkFlow.Extensions;

/// <summary>
/// 工作流平台服务扩展类。
/// </summary>
public static class PlatformExtension
{
    /// <summary>
    /// 异步创建系统工作流。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">业务工作流信息。</param>
    /// <returns></returns>
    public static Task CreateFlowAsync(this IPlatformService platform, Database db, FlowBizInfo info)
    {
        return FlowService.CreateFlowAsync(db, info);
    }

    /// <summary>
    /// 异步删除工作流及其日志。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">工作流业务数据ID。</param>
    /// <returns></returns>
    public static Task DeleteFlowAsync(this IPlatformService platform, Database db, string bizId)
    {
        return FlowService.DeleteFlowAsync(db, bizId);
    }

    /// <summary>
    /// 异步添加工作流日志信息。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">工作流业务数据ID。</param>
    /// <param name="stepName">流程步骤名称。</param>
    /// <param name="result">流程操作结果。</param>
    /// <param name="note">操作备注。</param>
    /// <param name="time">操作时间。</param>
    /// <returns></returns>
    public static Task AddFlowLogAsync(this IPlatformService platform, Database db, string bizId, string stepName, string result, string note, DateTime? time = null)
    {
        return FlowService.AddFlowLogAsync(db, bizId, stepName, result, note, time);
    }
}