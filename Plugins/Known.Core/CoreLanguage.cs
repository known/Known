namespace Known;

class CoreLanguage
{
    internal const string ConnectSuccess = "连接成功！";
    internal const string MigrateSuccess = "迁移成功！";

    internal const string TipAdminNoMigrate = "Admin插件不支持迁移！";
    internal const string TipSystemNotInstall = "系统未安装！";
    internal const string TipFileNotExists = "文件不存在！";
    internal const string TipModuleNotExists = "模块不存在！";
    internal const string TipModuleDeleteExistsChild = "存在子模块，不能删除！";
    internal const string TipInstallRequired = "安装信息不能为空！";
    internal const string TipNotNetwork = "电脑未联网！";
    internal const string TipTemplateTips = "提示：红色栏位为必填栏位！";
    internal const string TipTemplateFill = "填写：{text}";
    
    internal const string TipLoginTimeout = "用户登录已过期！";
    internal const string TipNoLogin = "用户未登录！";
    internal const string TipLoginNoNamePwd = "用户名或密码不正确！";
    internal const string TipLoginDisabled = "用户已禁用！";
    internal const string TipCurPwdRequired = "当前密码不能为空！";
    internal const string TipNewPwdRequired = "新密码不能为空！";
    internal const string TipConPwdRequired = "确认新密码不能为空！";
    internal const string TipUserDefaultPwd = "用户默认密码为：{password}。";
    internal const string TipModuleMigrated = "已经迁移过，无需再次迁移！";

    internal const string TipTaskInfo = "执行时间：{createTime}，耗时：{time}毫秒";
    internal const string TipTaskPending = "任务等待中...";
    internal const string TipTaskRunning = "任务执行中...";
    internal const string TipTaskAddSuccess = "任务添加成功，请稍后查询结果！";
    internal const string ImportFileImporting = "等待后台导入中...";
    internal const string ImportTaskPending = "导入任务等待中...";
    internal const string ImportTaskRunning = "导入任务执行中...";
}