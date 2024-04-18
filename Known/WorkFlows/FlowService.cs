namespace Known.WorkFlows;

class FlowService(Context context) : ServiceBase(context)
{
    private string FlowNotCreated => Language["Tip.FlowNotCreate"];
    private string UserNotExists(string user) => Language["Tip.UserNotExists"].Replace("{user}", user);
    private string NotExecuteFlow(string user) => Language["Tip.NotExecuteFlow"].Replace("{user}", user);

    internal static Task<SysFlow> GetFlowAsync(Database db, string bizId) => FlowRepository.GetFlowAsync(db, bizId);

    internal async Task<UserInfo> GetFlowStepUserAsync(Database db, string flowCode, string stepCode)
    {
        var user = CurrentUser;
        return await FlowRepository.GetFlowStepUserAsync(db, user.CompNo, user.AppId, flowCode, stepCode);
    }

    public Task<List<SysFlow>> GetFlowTodosAsync() => FlowRepository.GetFlowTodosAsync(Database);

    public Task<List<SysFlowLog>> GetFlowLogsAsync(string bizId) => FlowRepository.GetFlowLogsAsync(Database, bizId);

    public async Task<Result> SubmitFlowAsync(FlowFormInfo info)
    {
        var flows = await FlowRepository.GetFlowsAsync(Database, info.BizId);
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
                    Check.Throw(NotExecuteFlow(flow.CurrBy));

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(Context, flow);
                var result = await biz.OnCommitingAsync(db, info);
                if (!result.IsValid)
                    Check.Throw(result.Message);

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

        var flows = await FlowRepository.GetFlowsAsync(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error(FlowNotCreated);

        var user = CurrentUser;
        var name = Language.Revoke;
        return await Database.TransactionAsync(name, async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.PrevBy != user.UserName)
                    Check.Throw(NotExecuteFlow(flow.CurrBy));

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(Context, flow);
                var result = await biz.OnRevokingAsync(db, info);
                if (!result.IsValid)
                    Check.Throw(result.Message);

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
        var flows = await FlowRepository.GetFlowsAsync(Database, info.BizId);
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
                    Check.Throw(NotExecuteFlow(flow.CurrBy));

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

        var flows = await FlowRepository.GetFlowsAsync(Database, info.BizId);
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
                    Check.Throw(NotExecuteFlow(flow.CurrBy));

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(Context, flow);
                var result = await biz.OnVerifingAsync(db, info);
                if (!result.IsValid)
                    Check.Throw(result.Message);

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

        var flows = await FlowRepository.GetFlowsAsync(Database, info.BizId);
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
                    Check.Throw(result.Message);

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

        var flows = await FlowRepository.GetFlowsAsync(Database, info.BizId);
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
                    Check.Throw(result.Message);

                flow.BizStatus = FlowStatus.Stop;
                flow.FlowStatus = FlowStatus.Stop;
                await db.SaveAsync(flow);
                await AddFlowLogAsync(db, flow.BizId, FlowStatus.StepStopped, name, info.Note);
                info.FlowStatus = flow.FlowStatus;
                await biz.OnStoppedAsync(db, info);
            }
        });
    }

    internal async Task CreateFlowAsync(Database db, FlowBizInfo info)
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
        await AddFlowLogAsync(db, info.BizId, stepName, Language["Start"], info.BizName);
    }

    internal async Task DeleteFlowAsync(Database db, string bizId)
    {
        await FlowRepository.DeleteFlowLogsAsync(db, bizId);
        await FlowRepository.DeleteFlowAsync(db, bizId);
    }

    internal Task AddFlowLogAsync(Database db, string bizId, string stepName, string result, string note)
    {
        return db.SaveAsync(new SysFlowLog
        {
            Id = Utils.GetGuid(),
            CompNo = db.User.CompNo,
            AppId = db.User.AppId,
            BizId = bizId,
            StepName = stepName,
            ExecuteBy = db.User.Name,
            ExecuteTime = DateTime.Now,
            Result = result,
            Note = note
        });
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