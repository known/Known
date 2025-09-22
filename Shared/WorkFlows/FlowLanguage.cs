namespace Known;

class FlowLanguage
{
    internal const string Start = "开始";
    internal const string Assign = "分配";
    internal const string Verify = "审核";
    internal const string Pass = "通过";
    internal const string Fail = "退回";
    internal const string End = "结束";
    internal const string Restart = "重启";
    internal const string Stop = "停止";

    internal const string FlowOpen = "开启";
    internal const string FlowOver = "结束";
    internal const string FlowStop = "终止";
    internal const string FlowCreate = "创建流程";
    internal const string FlowSubmit = "提交流程";
    internal const string FlowRevoke = "撤回流程";
    internal const string FlowVerify = "审核流程";
    internal const string FlowAssign = "任务分配";
    internal const string FlowStopped = "终止流程";
    internal const string FlowRestart = "重启流程";
    internal const string FlowEnd = "结束流程";
    internal const string FlowSave = "暂存";
    internal const string FlowRevoked = "已撤回";
    internal const string FlowVerifing = "待审核";
    internal const string FlowPass = "审核通过";
    internal const string FlowFail = "审核退回";
    internal const string FlowReapply = "重新申请";

    internal const string SubmitToUser = "提交给[{user}]";
    internal const string AssignToUser = "指派给[{user}]";
    internal const string ReturnToUser = "退回给[{user}]";

    internal const string TipNotRegisterFlow = "流程未注册！";
    internal const string TipFlowNotCreate = "流程未创建，无法执行！";
    internal const string TipFlowDeleteSave = "只能删除暂存状态的记录！";
    internal const string TipUserNotExists = "账号[{user}]不存在！";
    internal const string TipNextUserNotExists = "下一步执行人[{user}]不存在！";
    internal const string TipNotExecuteFlow = "无权操作[{user}]的单据！";
    internal const string TipRevokeReason = "撤回原因不能为空！";
    internal const string TipReturnReason = "退回原因不能为空！";
    internal const string TipRestartReason = "重启原因不能为空！";
    internal const string TipStopReason = "终止原因不能为空！";
}