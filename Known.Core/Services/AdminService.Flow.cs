namespace Known.Services;

partial class AdminService
{
    private const string StepSubmit = "Submit";
    private const string StepRevoke = "Revoke";
    private const string StepVerify = "Verify";
    private const string StepAssign = "Assign";
    private const string StepStopped = "Stopped";
    private const string StepRestart = "Restart";
    private const string StepEnd = "End";

    private string FlowNotCreated => CoreLanguage.TipFlowNotCreate;
    private string UserNotExists(string user) => Language[CoreLanguage.TipUserNotExists].Replace("{user}", user);
    private string NotExecuteFlow(string user) => Language[CoreLanguage.TipNotExecuteFlow].Replace("{user}", user);

    public Task<PagingResult<FlowLogInfo>> QueryFlowLogsAsync(PagingCriteria criteria)
    {
        return Database.Query<SysFlowLog>(criteria).ToPageAsync<FlowLogInfo>();
    }

    public async Task<FlowInfo> GetFlowAsync(string moduleId, string bizId)
    {
        if (string.IsNullOrWhiteSpace(bizId))
            return new FlowInfo();

        var database = Database;
        var info = await GetFlowByModuleIdAsync(database, moduleId);
        if (info == null)
            return new FlowInfo();

        if (string.IsNullOrWhiteSpace(bizId))
            return info;

        var last = await database.Query<SysFlowLog>().Where(d => d.BizId == bizId)
                                 .OrderByDescending(d => d.CreateTime).FirstAsync();
        if (last != null && last.StepName == StepEnd && info.Steps.Count > 0)
            info.Current = info.Steps.Count - 1;
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
        var database = Database;
        var flows = await GetFlowsAsync(database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        var next = await database.GetUserInfoAsync(info.User);
        if (next == null)
            return Result.Error(UserNotExists(info.User));

        var user = CurrentUser;
        var name = Language.Submit;
        return await database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.CurrBy != user.UserName)
                    throw new SystemException(NotExecuteFlow(flow.CurrBy));

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = FlowBase.Create(Context, flow);
                var result = await biz.OnCommitingAsync(db, info);
                if (!result.IsValid)
                    throw new SystemException(result.Message);

                SetCurrToPrevStep(flow);
                SetCurrStep(flow, StepSubmit, next);

                var noteText = Language[CoreLanguage.SubmitToUser].Replace("{user}", flow.CurrBy);
                if (!string.IsNullOrEmpty(info.Note))
                    noteText += $"，{info.Note}";

                flow.BizStatus = info.BizStatus ?? FlowStatus.Verifing;
                flow.ApplyBy = user.UserName;
                flow.ApplyTime = DateTime.Now;
                await db.SaveAsync(flow);
                await db.AddFlowLogAsync(flow.BizId, StepSubmit, name, noteText);
                await biz.OnCommitedAsync(db, info);
            }
        });
    }

    public async Task<Result> RevokeFlowAsync(FlowFormInfo info)
    {
        if (string.IsNullOrEmpty(info.Note))
            return Result.Error(CoreLanguage.TipRevokeReason);

        var database = Database;
        var flows = await GetFlowsAsync(database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        var user = CurrentUser;
        var name = Language.Revoke;
        return await database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.PrevBy != user.UserName)
                    throw new SystemException(NotExecuteFlow(flow.CurrBy));

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = FlowBase.Create(Context, flow);
                var result = await biz.OnRevokingAsync(db, info);
                if (!result.IsValid)
                    throw new SystemException(result.Message);

                SetCurrToNextStep(flow);
                SetPrevToCurrStep(flow);
                flow.BizStatus = info.BizStatus ?? FlowStatus.Revoked;
                await db.SaveAsync(flow);
                await db.AddFlowLogAsync(flow.BizId, StepRevoke, name, info.Note);
                await biz.OnRevokedAsync(db, info);
            }
        });
    }

    public async Task<Result> AssignFlowAsync(FlowFormInfo info)
    {
        var database = Database;
        var flows = await GetFlowsAsync(database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        var next = await database.GetUserInfoAsync(info.User);
        if (next == null)
            return Result.Error(Language[CoreLanguage.TipNextUserNotExists].Replace("{user}", info.User));

        var user = CurrentUser;
        var name = CoreLanguage.Assign;
        return await database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.CurrBy != user.UserName)
                    throw new SystemException(NotExecuteFlow(flow.CurrBy));

                var stepName = flow.CurrStep;
                SetCurrToPrevStep(flow);
                SetCurrStep(flow, stepName, next);

                var noteText = Language[CoreLanguage.AssignToUser].Replace("{user}", flow.CurrBy);
                if (!string.IsNullOrEmpty(info.Note))
                    noteText += $"，{info.Note}";

                await db.SaveAsync(flow);
                await db.AddFlowLogAsync(flow.BizId, StepAssign, name, noteText);
            }
        });
    }

    public async Task<Result> VerifyFlowAsync(FlowFormInfo info)
    {
        var isPass = info.BizStatus == FlowStatus.VerifyPass;
        if (!isPass && string.IsNullOrEmpty(info.Note))
            return Result.Error(CoreLanguage.TipReturnReason);

        var database = Database;
        var flows = await GetFlowsAsync(database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        UserInfo next = null;
        if (isPass && !string.IsNullOrWhiteSpace(info.User))
        {
            next = await database.GetUserInfoAsync(info.User);
            if (next == null)
                return Result.Error(UserNotExists(info.User));
        }

        var user = CurrentUser;
        var name = CoreLanguage.Verify;
        return await database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.CurrBy != user.UserName)
                    throw new SystemException(NotExecuteFlow(flow.CurrBy));

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = FlowBase.Create(Context, flow);
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
                        await db.AddFlowLogAsync(flow.BizId, StepVerify, Language[CoreLanguage.Pass], info.Note);
                    }
                    else
                    {
                        flow.CurrBy = flow.ApplyBy;
                        flow.FlowStatus = FlowStatus.Over;
                        await db.SaveAsync(flow);
                        await db.AddFlowLogAsync(flow.BizId, StepVerify, Language[CoreLanguage.Pass], info.Note);
                        await db.AddFlowLogAsync(flow.BizId, StepEnd, Language[CoreLanguage.End], "");
                    }
                }
                else
                {
                    SetCurrToNextStep(flow);
                    SetPrevToCurrStep(flow);

                    var noteText = Language[CoreLanguage.ReturnToUser].Replace("{user}", flow.CurrBy);
                    if (!string.IsNullOrEmpty(info.Note))
                        noteText += $"，{info.Note}";

                    await db.SaveAsync(flow);
                    await db.AddFlowLogAsync(flow.BizId, StepVerify, Language[CoreLanguage.Fail], noteText);
                }
                info.FlowStatus = flow.FlowStatus;
                await biz.OnVerifiedAsync(db, info);
            }
        });
    }

    public async Task<Result> RepeatFlowAsync(FlowFormInfo info)
    {
        if (string.IsNullOrEmpty(info.Note))
            return Result.Error(CoreLanguage.TipRestartReason);

        var database = Database;
        var flows = await GetFlowsAsync(database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        var name = CoreLanguage.Restart;
        return await database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = FlowBase.Create(Context, flow);
                var result = await biz.OnRepeatingAsync(db, info);
                if (!result.IsValid)
                    throw new SystemException(result.Message);

                flow.BizStatus = info.BizStatus ?? FlowStatus.Reapply;
                flow.FlowStatus = FlowStatus.Open;
                await db.SaveAsync(flow);
                await db.AddFlowLogAsync(flow.BizId, StepRestart, name, info.Note);
                info.FlowStatus = flow.FlowStatus;
                await biz.OnRepeatedAsync(db, info);
            }
        });
    }

    public async Task<Result> StopFlowAsync(FlowFormInfo info)
    {
        if (string.IsNullOrEmpty(info.Note))
            return Result.Error(CoreLanguage.TipStopReason);

        var database = Database;
        var flows = await GetFlowsAsync(database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        var name = CoreLanguage.Stop;
        return await database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = FlowBase.Create(Context, flow);
                var result = await biz.OnStoppingAsync(db, info);
                if (!result.IsValid)
                    throw new SystemException(result.Message);

                flow.BizStatus = FlowStatus.Stop;
                flow.FlowStatus = FlowStatus.Stop;
                await db.SaveAsync(flow);
                await db.AddFlowLogAsync(flow.BizId, StepStopped, name, info.Note);
                info.FlowStatus = flow.FlowStatus;
                await biz.OnStoppedAsync(db, info);
            }
        });
    }

    private static async Task<FlowInfo> GetFlowByModuleIdAsync(Database db, string moduleId)
    {
        var param = await db.GetAutoPageAsync(moduleId, "");
        return DataHelper.ToFlow(param?.FlowData);
    }

    private static Task<List<SysFlow>> GetFlowsAsync(Database db, string bizIds)
    {
        if (string.IsNullOrWhiteSpace(bizIds))
            return Task.FromResult(new List<SysFlow>());

        var ids = bizIds.Split(',');
        return db.QueryListAsync<SysFlow>(d => ids.Contains(d.BizId));
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