namespace Known;

/// <summary>
/// 身份认证方式枚举。
/// </summary>
public enum AuthMode
{
    /// <summary>
    /// 浏览器Session。
    /// </summary>
    Session,
    /// <summary>
    /// 浏览器Cookie。
    /// </summary>
    Cookie,
    /// <summary>
    /// 微软Identity。
    /// </summary>
    Identity
}

/// <summary>
/// 框架后端配置选项类。
/// </summary>
public class CoreOption
{
    /// <summary>
    /// 取得App配置信息。
    /// </summary>
    public AppInfo App => Config.App;

    /// <summary>
    /// 取得或设置身份认证方式，默认浏览器Session。
    /// </summary>
    public AuthMode AuthMode { get; set; } = AuthMode.Session;

    /// <summary>
    /// 取得或设置响应数据是否启用压缩，默认禁用。
    /// </summary>
    public bool IsCompression { get; set; }

    /// <summary>
    /// 取得或设置是否根据Service动态生成WebApi，默认启用。
    /// </summary>
    public bool IsAddWebApi { get; set; } = true;
}