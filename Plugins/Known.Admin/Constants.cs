namespace Known;

class Constant
{
    /// <summary>
    /// 系统信息配置键。
    /// </summary>
    internal const string KeySystem = "SystemInfo";

    //internal const string UTOperation = "Operation";
    internal const string UMTypeReceive = "Receive";
    internal const string UMTypeSend = "Send";
    internal const string UMTypeDelete = "Delete";
    internal const string UMLGeneral = "General";
    internal const string UMLUrgent = "Urgent";
    internal const string UMStatusRead = "Read";
    internal const string UMStatusUnread = "Unread";
    //internal const string NSPublished = "Published";
}

/// <summary>
/// 系统定时任务状态类，代码表，类别是类名称。
/// </summary>
[CodeInfo]
public class SysTaskStatus
{
    private SysTaskStatus() { }

    /// <summary>
    /// 待执行。
    /// </summary>
    public const string Pending = "Pending";

    /// <summary>
    /// 执行中。
    /// </summary>
    public const string Running = "Running";

    /// <summary>
    /// 执行成功。
    /// </summary>
    public const string Success = "Success";

    /// <summary>
    /// 执行失败。
    /// </summary>
    public const string Failed = "Failed";
}

/// <summary>
/// 正则表达式匹配类。
/// </summary>
public class RegexPattern
{
    private RegexPattern() { }

    /// <summary>
    /// 中文正则匹配。
    /// </summary>
    public const string Chinese = "^[\u4e00-\u9fa5]{0,}$";

    /// <summary>
    /// 固定电话正则匹配。
    /// </summary>
    public const string Phone = "^0\\d{2,3}-[1-9]\\d{6,7}$";

    /// <summary>
    /// 手机号正则匹配。
    /// </summary>
    public const string Mobile = "^1[3-9]\\d{9}$";

    /// <summary>
    /// Email正则匹配。
    /// </summary>
    public const string Email = "^[A-Za-z0-9\\u4e00-\\u9fa5]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$";

    /// <summary>
    /// 网址正则匹配。
    /// </summary>
    public const string Url = "^(ht|f)tp(s?)\\:\\/\\/[0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*(:(0-9)*)*(\\/?)([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&%\\$#_]*)?$";
}