namespace Known;

/// <summary>
/// 系统上下文类。
/// </summary>
public class Context
{
    private Language language;
    private string currentLanguage;

    /// <summary>
    /// 构造函数，创建一个系统上下文类的实例。
    /// </summary>
    public Context() { }

    internal Context(string cultureName)
    {
        language = new Language(cultureName);
    }

    /// <summary>
    /// 取得上下文当前用户信息实例。
    /// </summary>
    public UserInfo CurrentUser { get; set; }

    /// <summary>
    /// 取得或设置当前语言标准编码，如：zh-CN/zh-TW/en-US。
    /// </summary>
    public string CurrentLanguage
    {
        get { return currentLanguage; }
        set
        {
            currentLanguage = value;
            language = new Language(value);
            var culture = new CultureInfo(language.Name);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }

    /// <summary>
    /// 取得当前语言对象。
    /// </summary>
    public Language Language
    {
        get
        {
            language ??= new Language(CurrentLanguage);
            return language;
        }
    }

    /// <summary>
    /// 创建一个上下文对象实例。
    /// </summary>
    /// <param name="token">用户Token。</param>
    /// <returns>上下文对象实例。</returns>
    public static Context Create(string token) => new()
    {
        CurrentUser = AuthService.GetUserByToken(token)
    };
}