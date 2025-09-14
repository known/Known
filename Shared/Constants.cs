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
    /// 用户设置配置键。
    /// </summary>
    public const string UserSetting = "UserSetting";

    /// <summary>
    /// 系统用户名。
    /// </summary>
    public const string SysUserName = "Admin";

    /// <summary>
    /// 超级管理员。
    /// </summary>
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>
    /// 基础数据菜单。
    /// </summary>
    public const string BaseData = "BaseData";

    /// <summary>
    /// 系统管理菜单。
    /// </summary>
    public const string System = "System";

    /// <summary>
    /// 系统通知Hub地址。
    /// </summary>
    public const string NotifyHubUrl = "/notifyHub";

    /// <summary>
    /// 通知到模板页。
    /// </summary>
    public const string NotifyLayout = "NotifyLayout";

    /// <summary>
    /// 系统信息配置键。
    /// </summary>
    public const string KeySystem = "SystemInfo";

    internal const string CompNo = "puman";
    internal const string CompName = "普漫科技";
    internal const string Route = "Route";
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