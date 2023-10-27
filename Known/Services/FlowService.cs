namespace Known.Services;

class FlowService : BaseService
{
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
            return Result.Error("流程未创建，无法执行！");

        var next = await UserService.GetUserAsync(Database, info.User);
        if (next == null)
            return Result.Error($"账号[{info.User}]不存在！");

        var user = CurrentUser;
        return await Database.TransactionAsync("提交", async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.CurrBy != user.UserName)
                    Check.Throw($"无权操作[{flow.CurrBy}]的单据！");

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(flow);
                var result = await biz.OnCommitingAsync(db, info);
                if (!result.IsValid)
                    Check.Throw(result.Message);

                SetCurrToPrevStep(flow);
                SetCurrStep(flow, "提交流程", next);

                var noteText = $"提交给[{flow.CurrBy}]";
                if (!string.IsNullOrEmpty(info.Note))
                    noteText += $"，{info.Note}";

                flow.BizStatus = info.BizStatus ?? FlowStatus.Verifing;
                flow.ApplyBy = user.UserName;
                flow.ApplyTime = DateTime.Now;
                await db.SaveAsync(flow);
                await AddFlowLogAsync(db, flow.BizId, "提交流程", Language.Submit, noteText);
                await biz.OnCommitedAsync(db, info);
            }
        });
    }

    public async Task<Result> RevokeFlowAsync(FlowFormInfo info)
    {
        if (string.IsNullOrEmpty(info.Note))
            return Result.Error("撤回原因不能为空！");

        var flows = await FlowRepository.GetFlowsAsync(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error("流程未创建，无法执行！");

        var user = CurrentUser;
        return await Database.TransactionAsync("撤回", async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.PrevBy != user.UserName)
                    Check.Throw($"无权撤回[{flow.PrevBy}]的单据！");

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(flow);
                var result = await biz.OnRevokingAsync(db, info);
                if (!result.IsValid)
                    Check.Throw(result.Message);

                SetCurrToNextStep(flow);
                SetPrevToCurrStep(flow);
                flow.BizStatus = info.BizStatus ?? FlowStatus.Revoked;
                await db.SaveAsync(flow);
                await AddFlowLogAsync(db, flow.BizId, "撤回流程", Language.Revoke, info.Note);
                await biz.OnRevokedAsync(db, info);
            }
        });
    }

    public async Task<Result> AssignFlowAsync(FlowFormInfo info)
    {
        var flows = await FlowRepository.GetFlowsAsync(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error("流程未创建，无法执行！");

        var next = await UserService.GetUserAsync(Database, info.User);
        if (next == null)
            return Result.Error($"下一步执行人[{info.User}]不存在！");

        var user = CurrentUser;
        return await Database.TransactionAsync("分配", async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.CurrBy != user.UserName)
                    Check.Throw($"无权操作[{flow.CurrBy}]的单据！");

                var stepName = flow.CurrStep;
                SetCurrToPrevStep(flow);
                SetCurrStep(flow, stepName, next);

                var noteText = $"指派给[{flow.CurrBy}]";
                if (!string.IsNullOrEmpty(info.Note))
                    noteText += $"，{info.Note}";

                await db.SaveAsync(flow);
                await AddFlowLogAsync(db, flow.BizId, "任务分配", "分配", noteText);
            }
        });
    }

    public async Task<Result> VerifyFlowAsync(FlowFormInfo info)
    {
        var isPass = info.BizStatus == FlowStatus.VerifyPass;
        if (!isPass && string.IsNullOrEmpty(info.Note))
            return Result.Error("退回原因不能为空！");

        var flows = await FlowRepository.GetFlowsAsync(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error("流程未创建，无法执行！");

        UserInfo next = null;
        if (isPass && !string.IsNullOrWhiteSpace(info.User))
        {
            next = await UserService.GetUserAsync(Database, info.User);
            if (next == null)
                return Result.Error($"账号[{info.User}]不存在！");
        }

        var user = CurrentUser;
        return await Database.TransactionAsync("审核", async db =>
        {
            foreach (var flow in flows)
            {
                if (flow.CurrBy != user.UserName)
                    Check.Throw($"无权操作[{flow.CurrBy}]的单据！");

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(flow);
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
                        await AddFlowLogAsync(db, flow.BizId, "审核流程", Language.Pass, info.Note);
                    }
                    else
                    {
                        flow.CurrBy = flow.ApplyBy;
                        flow.FlowStatus = FlowStatus.Over;
                        await db.SaveAsync(flow);
                        await AddFlowLogAsync(db, flow.BizId, "审核流程", Language.Pass, info.Note);
                        await AddFlowLogAsync(db, flow.BizId, "结束流程", "结束", "流程结束");
                    }
                }
                else
                {
                    SetCurrToNextStep(flow);
                    SetPrevToCurrStep(flow);

                    var noteText = $"退回给[{flow.CurrBy}]";
                    if (!string.IsNullOrEmpty(info.Note))
                        noteText += $"，{info.Note}";

                    await db.SaveAsync(flow);
                    await AddFlowLogAsync(db, flow.BizId, "审核流程", Language.Return, noteText);
                }
                info.FlowStatus = flow.FlowStatus;
                await biz.OnVerifiedAsync(db, info);
            }
        });
    }

    public async Task<Result> RepeatFlowAsync(FlowFormInfo info)
    {
        if (string.IsNullOrEmpty(info.Note))
            return Result.Error("重启原因不能为空！");

        var flows = await FlowRepository.GetFlowsAsync(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error("流程未创建，无法执行！");

        return await Database.TransactionAsync("重启", async db =>
        {
            foreach (var flow in flows)
            {
                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(flow);
                var result = await biz.OnRepeatingAsync(db, info);
                if (!result.IsValid)
                    Check.Throw(result.Message);

                flow.BizStatus = info.BizStatus ?? FlowStatus.ReApply;
                flow.FlowStatus = FlowStatus.Open;
                await db.SaveAsync(flow);
                await AddFlowLogAsync(db, flow.BizId, "重启流程", "重启", info.Note);
                info.FlowStatus = flow.FlowStatus;
                await biz.OnRepeatedAsync(db, info);
            }
        });
    }

    public async Task<Result> StopFlowAsync(FlowFormInfo info)
    {
        if (string.IsNullOrEmpty(info.Note))
            return Result.Error("终止原因不能为空！");

        var flows = await FlowRepository.GetFlowsAsync(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error("流程未创建，无法执行！");

        return await Database.TransactionAsync("终止", async db =>
        {
            foreach (var flow in flows)
            {
                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(flow);
                var result = await biz.OnStoppingAsync(db, info);
                if (!result.IsValid)
                    Check.Throw(result.Message);

                flow.BizStatus = FlowStatus.Stop;
                flow.FlowStatus = FlowStatus.Stop;
                await db.SaveAsync(flow);
                await AddFlowLogAsync(db, flow.BizId, "终止流程", "终止", info.Note);
                info.FlowStatus = flow.FlowStatus;
                await biz.OnStoppedAsync(db, info);
            }
        });
    }

    internal static Task AddFlowLogAsync(Database db, string bizId, string stepName, string result, string note)
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