namespace Known;

/// <summary>
/// 语言常量类。
/// </summary>
public partial class CoreLanguage
{
    internal const string Assign = "分配";
    internal const string Verify = "审核";
    internal const string Pass = "通过";
    internal const string Fail = "退回";
    internal const string Start = "开始";
    internal const string End = "结束";
    internal const string Restart = "重启";
    internal const string Stop = "停止";

    internal const string EntityPlugin = "实体插件";
    internal const string TableName = "数据表名";
    internal const string Fields = "字段列表";
    internal const string ConnectSuccess = "连接成功！";
    internal const string ExecuteSuccess = "执行成功！";
    internal const string MigrateSuccess = "迁移成功！";

    internal const string TipAdminNoMigrate = "Admin插件不支持迁移！";
    internal const string TipSystemNotInstall = "系统未安装！";
    internal const string TipFileNotExists = "文件不存在！";
    internal const string TipModuleNotExists = "模块不存在！";
    internal const string TipModuleDeleteExistsChild = "存在子模块，不能删除！";
    internal const string TipInstallRequired = "安装信息不能为空！";
    internal const string TipNotNetwork = "电脑未联网！";
    internal const string TipTableRequired = "实体表名不能为空！";
    internal const string TipTemplateTips = "提示：红色栏位为必填栏位！";
    internal const string TipTemplateFill = "填写：{text}";
    
    internal const string TipLoginTimeout = "用户登录已过期！";
    internal const string TipNoLogin = "用户未登录！";
    internal const string TipLoginNoNamePwd = "用户名或密码不正确！";
    internal const string TipLoginDisabled = "用户已禁用！";
    internal const string TipPathRequired = "路径不能为空！";
    internal const string TipCurPwdRequired = "当前密码不能为空！";
    internal const string TipNewPwdRequired = "新密码不能为空！";
    internal const string TipConPwdRequired = "确认新密码不能为空！";
    internal const string TipPwdNotEqual = "两次密码输入不一致！";
    internal const string TipUserDefaultPwd = "用户默认密码为：{password}。";
    internal const string TipNotSaveWithoutDev = "非开发环境，不能保存代码！";
    internal const string TipFileExisted = "文件[{file}]已存在！";
    internal const string TipModuleMigrated = "已经迁移过，无需再次迁移！";

    internal const string TipTaskInfo = "执行时间：{createTime}，耗时：{time}毫秒";
    internal const string TipTaskPending = "任务等待中...";
    internal const string TipTaskRunning = "任务执行中...";
    internal const string TipTaskAddSuccess = "任务添加成功，请稍后查询结果！";
    internal const string TipTableHasData = "数据表存在数据，不能执行！";
    internal const string ImportFileImporting = "等待后台导入中...";
    internal const string ImportTaskPending = "导入任务等待中...";
    internal const string ImportTaskRunning = "导入任务执行中...";

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
}