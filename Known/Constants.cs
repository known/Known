namespace Known;

public class Constants
{
    private Constants() { }

    internal const string DicCategory = "DicCategory";

    public const string KeyToken = "Known-Token";
    internal const string KeyClient = "Known-Client";
    internal const string KeyDownload = "Known-Download";
    internal const string SysUserName = "Admin";

    //internal const string UTOperation = "Operation";
    internal const string UMTypeReceive = "Receive";
    internal const string UMTypeSend = "Send";
    internal const string UMTypeDelete = "Delete";
    internal const string UMLGeneral = "General";
    internal const string UMLUrgent = "Urgent";
    internal const string UMStatusRead = "Read";
    internal const string UMStatusUnread = "Unread";
    //internal const string NSPublished = "Published";

    internal const string MimeImage = "image/jpeg,image/png";
    internal const string MimeVideo = "audio/mp4,video/mp4";
}

public class RegexPattern
{
    private RegexPattern() { }

    public const string Chinese = "^[\u4e00-\u9fa5]{0,}$";
    public const string Phone = "^0\\d{2,3}-[1-9]\\d{6,7}$";
    public const string Mobile = "^1[3-9]\\d{9}$";
    public const string Email = "^[A-Za-z0-9\\u4e00-\\u9fa5]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$";
    public const string Url = "^(ht|f)tp(s?)\\:\\/\\/[0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*(:(0-9)*)*(\\/?)([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&%\\$#_]*)?$";
}

[CodeInfo]
class TaskStatus
{
    private TaskStatus() { }

    public const string Pending = "Pending";
    public const string Running = "Running";
    public const string Success = "Success";
    public const string Failed = "Failed";
}