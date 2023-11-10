namespace Known;

public class Constants
{
    private Constants() { }

    internal const string DicCategory = "DicCategory";

    public const string KeyToken = "Known-Token";
    public const string KeyClient = "Known-Client";
    public const string KeyDownload = "Known-Download";
    public const string SysUserName = "Admin";

    public const string UTOperation = "运维人员";

    public const string UMTypeReceive = "收件";
    public const string UMTypeSend = "发件";
    public const string UMTypeDelete = "删除";
    public const string UMLGeneral = "普通";
    public const string UMLUrgent = "紧急";
    public const string UMStatusRead = "已读";
    public const string UMStatusUnread = "未读";

    public const string NSPublished = "已发布";

    public const string MimeImage = "image/jpeg,image/png";
    public const string MimeVideo = "audio/mp4,video/mp4";
}

public class RegexPattern
{
    public const string Chinese = "^[\u4e00-\u9fa5]{0,}$";
    public const string Phone = "^0\\d{2,3}-[1-9]\\d{6,7}$";
    public const string Mobile = "^1[3-9]\\d{9}$";
    public const string Email = "^[A-Za-z0-9\\u4e00-\\u9fa5]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$";
    public const string Url = "^(ht|f)tp(s?)\\:\\/\\/[0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*(:(0-9)*)*(\\/?)([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&%\\$#_]*)?$";
}

[CodeTable]
public class TaskStatus
{
    private TaskStatus() { }

    public const string Pending = "待执行";
    public const string Running = "执行中";
    public const string Success = "执行成功";
    public const string Failed = "执行失败";
}