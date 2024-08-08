namespace Known.WorkFlows;

public interface IFlowService : IService
{
    Task<PagingResult<SysFlowLog>> QueryFlowLogsAsync(PagingCriteria criteria);
    Task<FlowInfo> GetFlowAsync(string moduleId, string bizId);
    Task<Result> SubmitFlowAsync(FlowFormInfo info);
    Task<Result> RevokeFlowAsync(FlowFormInfo info);
    Task<Result> AssignFlowAsync(FlowFormInfo info);
    Task<Result> VerifyFlowAsync(FlowFormInfo info);
    Task<Result> RepeatFlowAsync(FlowFormInfo info);
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

        var logs = await Database.Query<SysFlowLog>().Where(d => d.BizId == bizId)
                                 .OrderBy(d => d.ExecuteTime).ToListAsync();
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

        //var ids = bizIds.Split(',');
        //return db.QueryListAsync<SysFlow>(d => d.BizId.In(ids));
        var ids = bizIds.Replace(",", "','");
        var sql = $"select * from SysFlow where BizId in ('{ids}')";
        return db.QueryListAsync<SysFlow>(sql);
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