namespace Known;

/// <summary>
/// 日志类型。
/// </summary>
public enum LogType
{
    /// <summary>
    /// 登录。
    /// </summary>
    Login,
    /// <summary>
    /// APP登录。
    /// </summary>
    AppLogin,
    /// <summary>
    /// 退出。
    /// </summary>
    Logout,
    /// <summary>
    /// 页面访问。
    /// </summary>
    Page,
    /// <summary>
    /// 操作。
    /// </summary>
    Operate,
    /// <summary>
    /// 其他。
    /// </summary>
    Other
}

/// <summary>
/// 系统日志信息类。
/// </summary>
public class LogInfo
{
    /// <summary>
    /// 取得或设置操作类型。
    /// </summary>
    public LogType Type { get; set; }

    /// <summary>
    /// 取得或设置操作对象。
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置操作内容。
    /// </summary>
    public string Content { get; set; }
}