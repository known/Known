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

    /// <summary>
    /// 构造函数，创建一个系统上下文类的实例。
    /// </summary>
    /// <param name="cultureName">本地化名称。</param>
    public Context(string cultureName)
    {
        language = new Language(cultureName);
    }

    /// <summary>
    /// 取得或设置注入的平台服务实例。
    /// </summary>
    internal IAdminService Admin { get; set; }

    /// <summary>
    /// 取得或设置上下文当前用户信息实例。
    /// </summary>
    public UserInfo CurrentUser { get; set; }

    /// <summary>
    /// 取得或设置是否是移动端。
    /// </summary>
    public bool IsMobile { get; set; }

    /// <summary>
    /// 取得或设置当前请求IP地址。
    /// </summary>
    public string IPAddress { get; set; }

    /// <summary>
    /// 取得或设置上下文请求对象，用于静态组件与后端交互。
    /// </summary>
    public IRequest Request { get; set; }

    /// <summary>
    /// 取得或设置上下文响应对象，用于静态组件与后端交互。
    /// </summary>
    public IResponse Response { get; set; }

    /// <summary>
    /// 取得当前是否启用移动端APP页面。
    /// </summary>
    public bool IsMobileApp => IsMobile && Config.App.IsMobile;

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
}

/// <summary>
/// 系统上下文请求接口，适用于静态组件与后端交互。
/// </summary>
public interface IRequest
{
    /// <summary>
    /// 取得请求方法，GET/POST。
    /// </summary>
    string Method { get; }

    /// <summary>
    /// 判断 POST 表单是否是指定请求处理者。
    /// </summary>
    /// <param name="name">处理者名称</param>
    /// <returns>是否指定请求处理者。</returns>
    bool IsHandler(string name);

    /// <summary>
    /// 根据控件名称获取指定泛型类型的数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="name">控件名称。</param>
    /// <returns>泛型类型的数据。</returns>
    T Get<T>(string name);

    /// <summary>
    /// 获取表单数据泛型类型对象。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>泛型类型对象。</returns>
    T GetModel<T>();
}

/// <summary>
/// 系统上下文响应接口，适用于静态组件与后端交互。
/// </summary>
public interface IResponse
{
    /// <summary>
    /// 重定向到指定页面。
    /// </summary>
    /// <param name="url">页面地址。</param>
    void Redirect(string url);
}