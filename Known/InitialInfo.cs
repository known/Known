namespace Known;

/// <summary>
/// 初始化数据信息类。
/// </summary>
public class InitialInfo
{
    /// <summary>
    /// 取得或设置系统主机地址或域名。
    /// </summary>
    public string HostUrl { get; set; }

    /// <summary>
    /// 取得或设置系统是否已经安装。
    /// </summary>
    public bool IsInstalled { get; set; }

    /// <summary>
    /// 取得或设置系统信息。
    /// </summary>
    public SystemInfo System { get; set; }

    /// <summary>
    /// 取得或设置多语言项目列表。
    /// </summary>
    public List<LanguageSettingInfo> LanguageSettings { get; set; }

    /// <summary>
    /// 取得或设置多语言数据列表。
    /// </summary>
    public List<SysLanguage> Languages { get; set; }

    /// <summary>
    /// 取得或设置客户端首页字典。
    /// </summary>
    public Dictionary<string, string> ClientHomes { get; set; } = [];

    /// <summary>
    /// 取得或设置系统相关配置信息。
    /// </summary>
    public Dictionary<string, object> Settings { get; set; } = [];
}