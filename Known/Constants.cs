namespace Known;

public class Constants
{
    private Constants() { }

    public const string KeyToken = "Known-Token";
    public const string KeyClient = "Known-Client";
    public const string KeyDownload = "Known-Download";
    public const string SysUserName = "Admin";

    public const string TaskPending = "待执行";
    public const string TaskRunning = "执行中";
    public const string TaskSuccess = "执行成功";
    public const string TaskFailed = "执行失败";

    public const string UMTypeReceive = "收件";
    public const string UMTypeSend = "发件";
    public const string UMTypeDelete = "删除";
    public const string UMLGeneral = "普通";
    public const string UMLUrgent = "紧急";
    public const string UMStatusRead = "已读";
    public const string UMStatusUnread = "未读";

    public const string NSPublished = "已发布";

    public const string LogTypeLogin = "登录";
    public const string LogTypeLogout = "退出";
    public const string LogTypePage = "页面";

    public const string MimeImage = "image/jpeg,image/png";
    public const string MimeVideo = "audio/mp4,video/mp4";
}

public class FlowStatus
{
    private FlowStatus() { }

    public const string Open = "开启";
    public const string Over = "结束";
    public const string Stop = "终止";

    public const string Save = "暂存";
    public const string Revoked = "已撤回";
    public const string Verifing = "待审核";
    public const string VerifyPass = "审核通过";
    public const string VerifyFail = "审核退回";
    public const string ReApply = "重新申请";
}