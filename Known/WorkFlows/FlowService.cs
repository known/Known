namespace Known.WorkFlows;

/// <summary>
/// 工作流服务接口。
/// </summary>
public interface IFlowService : IService
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

class FlowService(Context context) : ServiceBase(context), IFlowService
{
    private string FlowNotCreated => Language["Tip.FlowNotCreate"];
    private string UserNotExists(string user) => Language["Tip.UserNotExists"].Replace("{user}", user);
    private string NotExecuteFlow(string user) => Language["Tip.NotExecuteFlow"].Replace("{user}", user);

    public Task<PagingResult<SysFlowLog>> QueryFlowLogsAsync(PagingCriteria criteria)
    {
        return Database.QueryPageAsync<SysFlowLog>(criteria);
    }

    public async Task<FlowInfo> GetFlowAsync(string moduleId, string bizId)
    {
        var module = await Database.QueryByIdAsync<SysModule>(moduleId);
        var info = DataHelper.GetFlow(module?.FlowData);
        if (info == null)
            return new FlowInfo();

        var logs = await Repository.GetFlowLogsAsync(Database, bizId);
        if (logs != null && logs.Count > 0)
        {
            var last = logs.OrderByDescending(l => l.CreateTime).FirstOrDefault();
            if (last.StepName == FlowStatus.StepEnd && info.Steps.Count > 0)
                info.Current = info.Steps.Count - 1;
        }
        return info;
    }

    //internal static Task<SysFlow> GetFlowAsync(Database db, string bizId) => FlowRepository.GetFlowAsync(db, bizId);

    //internal async Task<UserInfo> GetFlowStepUserAsync(Database db, string flowCode, string stepCode)
    //{
    //    var user = CurrentUser;
    //    return await FlowRepository.GetFlowStepUserAsync(db, user.CompNo, user.AppId, flowCode, stepCode);
    //}

    //public Task<List<SysFlow>> GetFlowTodosAsync() => FlowRepository.GetFlowTodosAsync(Database);

    public async Task<Result> SubmitFlowAsync(FlowFormInfo info)
    {
        var flows = await GetFlowsAsync(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        var next = await AuthService.GetUserAsync(Database, info.User);
        if (next == null)
            return Result.Error(UserNotExists(info.User));

        var user = CurrentUser;
        var name = Language.Submit;
        return await Database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.CurrBy != user.UserName)
                    throw new SystemException(NotExecuteFlow(flow.CurrBy));

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(Context, flow);
                var result = await biz.OnCommitingAsync(db, info);
                if (!result.IsValid)
                    throw new SystemException(result.Message);

                SetCurrToPrevStep(flow);
                SetCurrStep(flow, FlowStatus.StepSubmit, next);

                var noteText = Language["SubmitToUser"].Replace("{user}", flow.CurrBy);
                if (!string.IsNullOrEmpty(info.Note))
                    noteText += $"，{info.Note}";

                flow.BizStatus = info.BizStatus ?? FlowStatus.Verifing;
                flow.ApplyBy = user.UserName;
                flow.ApplyTime = DateTime.Now;
                await db.SaveAsync(flow);
                await AddFlowLogAsync(db, flow.BizId, FlowStatus.StepSubmit, name, noteText);
                await biz.OnCommitedAsync(db, info);
            }
        });
    }

    public async Task<Result> RevokeFlowAsync(FlowFormInfo info)
    {
        if (string.IsNullOrEmpty(info.Note))
            return Result.Error(Language["Tip.RevokeReason"]);

        var flows = await GetFlowsAsync(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        var user = CurrentUser;
        var name = Language.Revoke;
        return await Database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.PrevBy != user.UserName)
                    throw new SystemException(NotExecuteFlow(flow.CurrBy));

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(Context, flow);
                var result = await biz.OnRevokingAsync(db, info);
                if (!result.IsValid)
                    throw new SystemException(result.Message);

                SetCurrToNextStep(flow);
                SetPrevToCurrStep(flow);
                flow.BizStatus = info.BizStatus ?? FlowStatus.Revoked;
                await db.SaveAsync(flow);
                await AddFlowLogAsync(db, flow.BizId, FlowStatus.StepRevoke, name, info.Note);
                await biz.OnRevokedAsync(db, info);
            }
        });
    }

    public async Task<Result> AssignFlowAsync(FlowFormInfo info)
    {
        var flows = await GetFlowsAsync(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        var next = await AuthService.GetUserAsync(Database, info.User);
        if (next == null)
            return Result.Error(Language["Tip.NextUserNotExists"].Replace("{user}", info.User));

        var user = CurrentUser;
        var name = Language["Button.Assign"];
        return await Database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.CurrBy != user.UserName)
                    throw new SystemException(NotExecuteFlow(flow.CurrBy));

                var stepName = flow.CurrStep;
                SetCurrToPrevStep(flow);
                SetCurrStep(flow, stepName, next);

                var noteText = Language["AssignToUser"].Replace("{user}", flow.CurrBy);
                if (!string.IsNullOrEmpty(info.Note))
                    noteText += $"，{info.Note}";

                await db.SaveAsync(flow);
                await AddFlowLogAsync(db, flow.BizId, FlowStatus.StepAssign, name, noteText);
            }
        });
    }

    public async Task<Result> VerifyFlowAsync(FlowFormInfo info)
    {
        var isPass = info.BizStatus == FlowStatus.VerifyPass;
        if (!isPass && string.IsNullOrEmpty(info.Note))
            return Result.Error(Language["Tip.ReturnReason"]);

        var flows = await GetFlowsAsync(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        UserInfo next = null;
        if (isPass && !string.IsNullOrWhiteSpace(info.User))
        {
            next = await AuthService.GetUserAsync(Database, info.User);
            if (next == null)
                return Result.Error(UserNotExists(info.User));
        }

        var user = CurrentUser;
        var name = Language["Button.Verify"];
        return await Database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.CurrBy != user.UserName)
                    throw new SystemException(NotExecuteFlow(flow.CurrBy));

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(Context, flow);
                var result = await biz.OnVerifingAsync(db, info);
                if (!result.IsValid)
                    throw new SystemException(result.Message);

                flow.BizStatus = info.BizStatus;
                flow.VerifyBy = user.UserName;
                flow.VerifyTime = DateTime.Now;
                flow.VerifyNote = info.Note;
                if (isPass)
                {
                    SetCurrToPrevStep(flow);

                    if (next != null)
                    {
                        flow.CurrBy = next.UserName;
                        await db.SaveAsync(flow);
                        await AddFlowLogAsync(db, flow.BizId, FlowStatus.StepVerify, Language["Pass"], info.Note);
                    }
                    else
                    {
                        flow.CurrBy = flow.ApplyBy;
                        flow.FlowStatus = FlowStatus.Over;
                        await db.SaveAsync(flow);
                        await AddFlowLogAsync(db, flow.BizId, FlowStatus.StepVerify, Language["Pass"], info.Note);
                        await AddFlowLogAsync(db, flow.BizId, FlowStatus.StepEnd, Language["End"], "");
                    }
                }
                else
                {
                    SetCurrToNextStep(flow);
                    SetPrevToCurrStep(flow);

                    var noteText = Language["ReturnToUser"].Replace("{user}", flow.CurrBy);
                    if (!string.IsNullOrEmpty(info.Note))
                        noteText += $"，{info.Note}";

                    await db.SaveAsync(flow);
                    await AddFlowLogAsync(db, flow.BizId, FlowStatus.StepVerify, Language["Fail"], noteText);
                }
                info.FlowStatus = flow.FlowStatus;
                await biz.OnVerifiedAsync(db, info);
            }
        });
    }

    public async Task<Result> RepeatFlowAsync(FlowFormInfo info)
    {
        if (string.IsNullOrEmpty(info.Note))
            return Result.Error(Language["Tip.RestartReason"]);

        var flows = await GetFlowsAsync(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        var name = Language["Button.Restart"];
        return await Database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(Context, flow);
                var result = await biz.OnRepeatingAsync(db, info);
                if (!result.IsValid)
                    throw new SystemException(result.Message);

                flow.BizStatus = info.BizStatus ?? FlowStatus.Reapply;
                flow.FlowStatus = FlowStatus.Open;
                await db.SaveAsync(flow);
                await AddFlowLogAsync(db, flow.BizId, FlowStatus.StepRestart, name, info.Note);
                info.FlowStatus = flow.FlowStatus;
                await biz.OnRepeatedAsync(db, info);
            }
        });
    }

    public async Task<Result> StopFlowAsync(FlowFormInfo info)
    {
        if (string.IsNullOrEmpty(info.Note))
            return Result.Error(Language["Tip.StopReason"]);

        var flows = await GetFlowsAsync(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        var name = Language["Button.Stop"];
        return await Database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(Context, flow);
                var result = await biz.OnStoppingAsync(db, info);
                if (!result.IsValid)
                    throw new SystemException(result.Message);

                flow.BizStatus = FlowStatus.Stop;
                flow.FlowStatus = FlowStatus.Stop;
                await db.SaveAsync(flow);
                await AddFlowLogAsync(db, flow.BizId, FlowStatus.StepStopped, name, info.Note);
                info.FlowStatus = flow.FlowStatus;
                await biz.OnStoppedAsync(db, info);
            }
        });
    }

    internal static async Task CreateFlowAsync(Database db, FlowBizInfo info)
    {
        var stepName = FlowStatus.StepCreate;
        var flow = new SysFlow
        {
            Id = Utils.GetGuid(),
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
        await AddFlowLogAsync(db, info.BizId, stepName, "Start", info.BizName);
    }

    internal static async Task DeleteFlowAsync(Database db, string bizId)
    {
        await db.DeleteAsync<SysFlowLog>(d => d.BizId == bizId);
        await db.DeleteAsync<SysFlow>(d => d.BizId == bizId);
    }

    internal static Task AddFlowLogAsync(Database db, string bizId, string stepName, string result, string note, DateTime? time = null)
    {
        return db.SaveAsync(new SysFlowLog
        {
            Id = Utils.GetGuid(),
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

    private static Task<List<SysFlow>> GetFlowsAsync(Database db, string bizIds)
    {
        if (string.IsNullOrWhiteSpace(bizIds))
            return null;

        var ids = bizIds.Split(',');
        return db.QueryListAsync<SysFlow>(d => d.BizId.In(ids));
    }

    private static void SetPrevToCurrStep(SysFlow info)
    {
        info.CurrStep = info.PrevStep;
        info.CurrBy = info.PrevBy;
    }

    private static void SetNextToCurrStep(SysFlow info)
    {
        info.CurrStep = info.NextStep;
        info.CurrBy = info.NextBy;
    }

    private static void SetCurrToPrevStep(SysFlow info)
    {
        info.PrevStep = info.CurrStep;
        info.PrevBy = info.CurrBy;
    }

    private static void SetCurrToNextStep(SysFlow info)
    {
        info.NextStep = info.CurrStep;
        info.NextBy = info.CurrBy;
    }

    private static void SetCurrStep(SysFlow info, string stepName, UserInfo user)
    {
        info.CurrStep = stepName;
        info.CurrBy = user.UserName;
    }
}