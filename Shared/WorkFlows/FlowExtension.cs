namespace Known.WorkFlows;

/// <summary>
/// 工作流数据扩展类。
/// </summary>
public static class FlowExtension
{
    private const string StepCreate = FlowLanguage.FlowCreate;

    /// <summary>
    /// 异步创建系统工作流。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">业务工作流信息。</param>
    /// <returns></returns>
    public static async Task CreateFlowAsync(this Database db, FlowBizInfo info)
    {
        var stepName = StepCreate;
        var flow = new SysFlow
        {
            CompNo = db.User.CompNo,
            AppId = db.User.AppId,
            FlowCode = info.FlowCode,
            FlowName = info.FlowName,
            FlowStatus = FlowStatus.Open,
            BizId = info.BizId,
            BizName = info.BizName,
            BizUrl = info.BizUrl,
            BizStatus = info.BizStatus,
            CurrStep = stepName,
            CurrBy = db.User.UserName
        };
        await db.SaveAsync(flow);
        await db.AddFlowLogAsync(info.BizId, stepName, FlowLanguage.Start, info.BizName);
    }

    /// <summary>
    /// 异步删除工作流及其日志。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">工作流业务数据ID。</param>
    /// <returns></returns>
    public static async Task DeleteFlowAsync(this Database db, string bizId)
    {
        await db.DeleteAsync<SysFlowLog>(d => d.BizId == bizId);
        await db.DeleteAsync<SysFlow>(d => d.BizId == bizId);
    }

    /// <summary>
    /// 异步添加工作流日志信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">工作流业务数据ID。</param>
    /// <param name="stepName">流程步骤名称。</param>
    /// <param name="result">流程操作结果。</param>
    /// <param name="note">操作备注。</param>
    /// <param name="time">操作时间。</param>
    /// <returns></returns>
    public static Task AddFlowLogAsync(this Database db, string bizId, string stepName, string result, string note, DateTime? time = null)
    {
        return db.SaveAsync(new SysFlowLog
        {
            CompNo = db.User.CompNo,
            AppId = db.User.AppId,
            BizId = bizId,
            StepName = stepName,
            ExecuteBy = db.User.Name,
            ExecuteTime = time ?? DateTime.Now,
            Result = result,
            Note = note
        });
    }
}