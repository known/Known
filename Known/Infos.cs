namespace Known;

/// <summary>
/// 初始化数据信息类。
/// </summary>
public class InitialInfo
{
    /// <summary>
    /// 取得或设置多语言项目列表。
    /// </summary>
    public List<LanguageSettingInfo> LanguageSettings { get; set; }

    /// <summary>
    /// 取得或设置多语言数据列表。
    /// </summary>
    public List<LanguageInfo> Languages { get; set; }
}