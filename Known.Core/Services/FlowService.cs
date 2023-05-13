namespace Known.Core.Services;

public class FlowService : BaseService
{
    internal FlowService(Context context) : base(context) { }

    internal static SysFlow GetFlow(Database db, string bizId) => FlowRepository.GetFlow(db, bizId);

    internal UserInfo GetFlowStepUser(Database db, string flowCode, string stepCode)
    {
        var user = CurrentUser;
        return FlowRepository.GetFlowStepUser(db, user.CompNo, user.AppId, flowCode, stepCode);
    }

    internal List<SysFlow> GetFlowTodos()
    {
        var user = CurrentUser;
        return FlowRepository.GetFlowTodos(Database, user.AppId, user.UserName);
    }

    internal List<SysFlowLog> GetFlowLogs(string bizId) => FlowRepository.GetFlowLogs(Database, bizId);

    public static void DeleteFlow(Database db, string bizId)
    {
        FlowRepository.DeleteFlowLogs(db, bizId);
        FlowRepository.DeleteFlow(db, bizId);
    }

    public static void CreateFlow(Database db, FlowBizInfo info)
    {
        var stepName = "创建流程";
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
            CurrBy = db.User.UserId
        };
        db.Save(flow);
        AddFlowLog(db, info.BizId, stepName, "创建", info.BizName);
    }

    internal Result SubmitFlow(FlowFormInfo info)
    {
        var flows = FlowRepository.GetFlows(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error("流程未创建，无法执行！");

        var next = UserService.GetUser(Database, info.User);
        if (next == null)
            return Result.Error($"账号[{info.User}]不存在！");

        var user = CurrentUser;
        return Database.Transaction("提交", db =>
        {
            foreach (var flow in flows)
            {
                if (flow.CurrBy != user.UserId)
                    Check.Throw($"无权操作[{flow.CurrBy}]的单据！");

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(flow);
                var result = biz.OnCommiting(db, info);
                if (!result.IsValid)
                    Check.Throw(result.Message);

                SetCurrToPrevStep(flow);

                if (!string.IsNullOrEmpty(flow.NextBy))
                    SetNextToCurrStep(flow);
                else
                    SetCurrStep(flow, "提交流程", next);

                var noteText = $"提交给[{flow.CurrBy}]";
                if (!string.IsNullOrEmpty(info.Note))
                    noteText += $"，{info.Note}";

                flow.BizStatus = info.BizStatus ?? FlowStatus.Verifing;
                flow.ApplyBy = user.UserId;
                flow.ApplyTime = DateTime.Now;
                db.Save(flow);
                AddFlowLog(db, flow.BizId, "提交流程", Language.Submit, noteText);
                biz.OnCommited(db, info);
            }
        });
    }

    internal Result RevokeFlow(FlowFormInfo info)
    {
        if (string.IsNullOrEmpty(info.Note))
            return Result.Error("撤回原因不能为空！");

        var flows = FlowRepository.GetFlows(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error("流程未创建，无法执行！");

        var user = CurrentUser;
        return Database.Transaction("撤回", db =>
        {
            foreach (var flow in flows)
            {
                if (flow.PrevBy != user.UserId)
                    Check.Throw($"无权撤回[{flow.PrevBy}]的单据！");

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(flow);
                var result = biz.OnRevoking(db, info);
                if (!result.IsValid)
                    Check.Throw(result.Message);

                SetCurrToNextStep(flow);
                SetPrevToCurrStep(flow);
                flow.BizStatus = info.BizStatus ?? FlowStatus.Revoked;
                db.Save(flow);
                AddFlowLog(db, flow.BizId, "撤回流程", Language.Revoke, info.Note);
                biz.OnRevoked(db, info);
            }
        });
    }

    internal Result AssignFlow(FlowFormInfo info)
    {
        var flows = FlowRepository.GetFlows(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error("流程未创建，无法执行！");

        var next = UserService.GetUser(Database, info.User);
        if (next == null)
            return Result.Error($"下一步执行人[{info.User}]不存在！");

        var user = CurrentUser;
        return Database.Transaction("分配", db =>
        {
            foreach (var flow in flows)
            {
                if (flow.CurrBy != user.UserId)
                    Check.Throw($"无权操作[{flow.CurrBy}]的单据！");

                var stepName = flow.CurrStep;
                SetCurrToPrevStep(flow);
                SetCurrStep(flow, stepName, next);

                var noteText = $"指派给[{flow.CurrBy}]";
                if (!string.IsNullOrEmpty(info.Note))
                    noteText += $"，{info.Note}";

                db.Save(flow);
                AddFlowLog(db, flow.BizId, "任务分配", "分配", noteText);
            }
        });
    }

    internal Result VerifyFlow(FlowFormInfo info)
    {
        var isPass = info.BizStatus == FlowStatus.VerifyPass;
        if (!isPass && string.IsNullOrEmpty(info.Note))
            return Result.Error("退回原因不能为空！");

        var flows = FlowRepository.GetFlows(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error("流程未创建，无法执行！");

        UserInfo next = null;
        if (isPass && !string.IsNullOrWhiteSpace(info.User))
        {
            next = UserService.GetUser(Database, info.User);
            if (next == null)
                return Result.Error($"账号[{info.User}]不存在！");
        }

        var user = CurrentUser;
        return Database.Transaction("审核", db =>
        {
            foreach (var flow in flows)
            {
                if (flow.CurrBy != user.UserId)
                    Check.Throw($"无权操作[{flow.CurrBy}]的单据！");

                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(flow);
                var result = biz.OnVerifing(db, info);
                if (!result.IsValid)
                    Check.Throw(result.Message);

                flow.BizStatus = info.BizStatus;
                flow.VerifyBy = user.UserId;
                flow.VerifyTime = DateTime.Now;
                if (isPass)
                {
                    SetCurrToPrevStep(flow);

                    if (next != null)
                    {
                        flow.CurrBy = next.UserId;
                        db.Save(flow);
                        AddFlowLog(db, flow.BizId, "审核流程", Language.Pass, info.Note);
                    }
                    else
                    {
                        flow.CurrBy = flow.ApplyBy;
                        flow.FlowStatus = FlowStatus.Over;
                        db.Save(flow);
                        AddFlowLog(db, flow.BizId, "审核流程", Language.Pass, info.Note);
                        AddFlowLog(db, flow.BizId, "结束流程", "结束", "流程结束");
                    }
                }
                else
                {
                    SetCurrToNextStep(flow);
                    SetPrevToCurrStep(flow);

                    var noteText = $"退回给[{flow.CurrBy}]";
                    if (!string.IsNullOrEmpty(info.Note))
                        noteText += $"，{info.Note}";

                    db.Save(flow);
                    AddFlowLog(db, flow.BizId, "审核流程", Language.Return, noteText);
                }
                info.FlowStatus = flow.FlowStatus;
                biz.OnVerified(db, info);
            }
        });
    }

    internal Result RepeatFlow(FlowFormInfo info)
    {
        if (string.IsNullOrEmpty(info.Note))
            return Result.Error("重启原因不能为空！");

        var flows = FlowRepository.GetFlows(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error("流程未创建，无法执行！");

        return Database.Transaction("重启", db =>
        {
            foreach (var flow in flows)
            {
                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(flow);
                var result = biz.OnRepeating(db, info);
                if (!result.IsValid)
                    Check.Throw(result.Message);

                flow.BizStatus = info.BizStatus ?? FlowStatus.ReApply;
                flow.FlowStatus = FlowStatus.Open;
                db.Save(flow);
                AddFlowLog(db, flow.BizId, "重启流程", "重启", info.Note);
                info.FlowStatus = flow.FlowStatus;
                biz.OnRepeated(db, info);
            }
        });
    }

    internal Result StopFlow(FlowFormInfo info)
    {
        if (string.IsNullOrEmpty(info.Note))
            return Result.Error("终止原因不能为空！");

        var flows = FlowRepository.GetFlows(Database, info.BizId);
        if (flows == null || flows.Count == 0)
            return Result.Error("流程未创建，无法执行！");

        return Database.Transaction("终止", db =>
        {
            foreach (var flow in flows)
            {
                info.BizId = flow.BizId;
                info.FlowStatus = flow.FlowStatus;
                var biz = BaseFlow.Create(flow);
                var result = biz.OnStopping(db, info);
                if (!result.IsValid)
                    Check.Throw(result.Message);

                flow.BizStatus = FlowStatus.Stop;
                flow.FlowStatus = FlowStatus.Stop;
                db.Save(flow);
                AddFlowLog(db, flow.BizId, "终止流程", "终止", info.Note);
                info.FlowStatus = flow.FlowStatus;
                biz.OnStopped(db, info);
            }
        });
    }

    public static void AddFlowLog(Database db, string bizId, string stepName, string result, string note)
    {
        db.Save(new SysFlowLog
        {
            Id = Utils.GetGuid(),
            CompNo = db.User.CompNo,
            AppId = db.User.AppId,
            BizId = bizId,
            StepName = stepName,
            ExecuteBy = db.User.UserId,
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
        info.CurrBy = user.UserId;
    }
}