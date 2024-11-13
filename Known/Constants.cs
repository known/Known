namespace Known;

/// <summary>
/// 常量类。
/// </summary>
public class Constants
{
    private Constants() { }

    /// <summary>
    /// 数据字典类别。
    /// </summary>
    public const string DicCategory = "DicCategory";

    /// <summary>
    /// 框架用户Token键。
    /// </summary>
    public const string KeyToken = "Known-Token";

    /// <summary>
    /// 框架用户认证键。
    /// </summary>
    public const string KeyAuth = "Known-Auth";

    internal const string KeyClient = "Known-Client";
    internal const string KeyDownload = "Known-Download";

    /// <summary>
    /// 系统用户名。
    /// </summary>
    public const string SysUserName = "Admin";

    /// <summary>
    /// 标识路由菜单类型。
    /// </summary>
    public const string Route = "Route";

    internal const string MimeImage = "image/jpeg,image/png";
    internal const string MimeVideo = "audio/mp4,video/mp4";
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