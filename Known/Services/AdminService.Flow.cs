namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步分页查询工作流程日志。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<SysFlowLog>> QueryFlowLogsAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取流程配置信息。
    /// </summary>
    /// <param name="moduleId">模块ID。</param>
    /// <param name="bizId">业务数据ID。</param>
    /// <returns>流程配置信息。</returns>
    Task<FlowInfo> GetFlowAsync(string moduleId, string bizId);

    /// <summary>
    /// 异步提交工作流。
    /// </summary>
    /// <param name="info">流程表单对象。</param>
    /// <returns>提交结果。</returns>
    Task<Result> SubmitFlowAsync(FlowFormInfo info);

    /// <summary>
    /// 异步撤回工作流。
    /// </summary>
    /// <param name="info">流程表单对象。</param>
    /// <returns>撤回结果。</returns>
    Task<Result> RevokeFlowAsync(FlowFormInfo info);

    /// <summary>
    /// 异步指派工作流。
    /// </summary>
    /// <param name="info">流程表单对象。</param>
    /// <returns>指派结果。</returns>
    Task<Result> AssignFlowAsync(FlowFormInfo info);

    /// <summary>
    /// 异步审核工作流。
    /// </summary>
    /// <param name="info">流程表单对象。</param>
    /// <returns>审核结果。</returns>
    Task<Result> VerifyFlowAsync(FlowFormInfo info);

    /// <summary>
    /// 异步重启工作流。
    /// </summary>
    /// <param name="info">流程表单对象。</param>
    /// <returns>重启结果。</returns>
    Task<Result> RepeatFlowAsync(FlowFormInfo info);

    /// <summary>
    /// 异步停止工作流。
    /// </summary>
    /// <param name="info">流程表单对象。</param>
    /// <returns>停止结果。</returns>
    Task<Result> StopFlowAsync(FlowFormInfo info);
}

partial class AdminService
{
    public Task<PagingResult<SysFlowLog>> QueryFlowLogsAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<SysFlowLog>());
    }

    public Task<FlowInfo> GetFlowAsync(string moduleId, string bizId)
    {
        return Task.FromResult(new FlowInfo());
    }

    public Task<Result> SubmitFlowAsync(FlowFormInfo info)
    {
        return Result.SuccessAsync("提交成功！");
    }

    public Task<Result> RevokeFlowAsync(FlowFormInfo info)
    {
        return Result.SuccessAsync("撤回成功！");
    }

    public Task<Result> AssignFlowAsync(FlowFormInfo info)
    {
        return Result.SuccessAsync("分配成功！");
    }

    public Task<Result> VerifyFlowAsync(FlowFormInfo info)
    {
        return Result.SuccessAsync("审核成功！");
    }

    public Task<Result> RepeatFlowAsync(FlowFormInfo info)
    {
        return Result.SuccessAsync("重启成功！");
    }

    public Task<Result> StopFlowAsync(FlowFormInfo info)
    {
        return Result.SuccessAsync("停止成功！");
    }
}

partial class AdminClient
{
    public Task<PagingResult<SysFlowLog>> QueryFlowLogsAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<SysFlowLog>("/Admin/QueryFlowLogs", criteria);
    }

    public Task<FlowInfo> GetFlowAsync(string moduleId, string bizId)
    {
        return Http.GetAsync<FlowInfo>($"/Admin/GetFlow?moduleId={moduleId}&bizId={bizId}");
    }

    public Task<Result> SubmitFlowAsync(FlowFormInfo info)
    {
        return Http.PostAsync("/Admin/SubmitFlow", info);
    }

    public Task<Result> RevokeFlowAsync(FlowFormInfo info)
    {
        return Http.PostAsync("/Admin/RevokeFlow", info);
    }

    public Task<Result> AssignFlowAsync(FlowFormInfo info)
    {
        return Http.PostAsync("/Admin/AssignFlow", info);
    }

    public Task<Result> VerifyFlowAsync(FlowFormInfo info)
    {
        return Http.PostAsync("/Admin/VerifyFlow", info);
    }

    public Task<Result> RepeatFlowAsync(FlowFormInfo info)
    {
        return Http.PostAsync("/Admin/RepeatFlow", info);
    }

    public Task<Result> StopFlowAsync(FlowFormInfo info)
    {
        return Http.PostAsync("/Admin/StopFlow", info);
    }
}