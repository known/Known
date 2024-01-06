namespace Known;

class Constants
{
    private Constants() { }

    internal const string DicCategory = "DicCategory";

    public const string KeyToken = "Known-Token";
    public const string KeyClient = "Known-Client";
    public const string KeyDownload = "Known-Download";
    public const string SysUserName = "Admin";

    //public const string UTOperation = "Operation";
    public const string UMTypeReceive = "Receive";
    public const string UMTypeSend = "Send";
    public const string UMTypeDelete = "Delete";
    public const string UMLGeneral = "General";
    public const string UMLUrgent = "Urgent";
    public const string UMStatusRead = "Read";
    public const string UMStatusUnread = "Unread";
    //public const string NSPublished = "Published";

    public const string MimeImage = "image/jpeg,image/png";
    public const string MimeVideo = "audio/mp4,video/mp4";
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