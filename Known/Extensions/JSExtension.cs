namespace Known.Extensions;

/// <summary>
/// JS运行时和服务扩展类。
/// </summary>
public static class JSExtension
{
    private static readonly string KeyUserInfo = "Known_User";
    private static readonly string KeySize = "Known_Size";
    private static readonly string KeyLanguage = "Known_Language";
    private static readonly string KeyTheme = "Known_Theme";

    /// <summary>
    /// 异步粘贴剪贴板里的数据。
    /// </summary>
    /// <param name="js">JS运行时。</param>
    /// <param name="action">粘贴数据处理委托。</param>
    /// <returns></returns>
    public static async Task PasteTextAsync(this IJSRuntime js, Action<string> action)
    {
        var text = await js.InvokeAsync<string>("navigator.clipboard.readText", null);
        action?.Invoke(text);
    }

    /// <summary>
    /// 异步自动填充页面表格高度。
    /// </summary>
    /// <param name="js">JS运行时。</param>
    /// <returns></returns>
    public static ValueTask FillTableHeightAsync(this IJSRuntime js)
    {
        return js.InvokeVoidAsync("K_AutoFillTableHeight");
    }

    /// <summary>
    /// 异步获取浏览器会话存储的当前用户信息。
    /// </summary>
    /// <param name="js">JS服务。</param>
    /// <returns>当前用户信息。</returns>
    public static Task<UserInfo> GetUserInfoAsync(this JSService js) => js.GetSessionStorageAsync<UserInfo>(KeyUserInfo);

    /// <summary>
    /// 异步存储当前用户信息到浏览器会话中。
    /// </summary>
    /// <param name="js">JS服务。</param>
    /// <param name="data">当前用户信息。</param>
    /// <returns></returns>
    public static Task SetUserInfoAsync(this JSService js, object data) => js.SetSessionStorageAsync(KeyUserInfo, data);

    internal static Task<string> GetCurrentSizeAsync(this JSService js) => js.GetLocalStorageAsync<string>(KeySize);
    internal static async Task SetCurrentSizeAsync(this JSService js, string size)
    {
        var item = UIConfig.Sizes.FirstOrDefault(s => s.Id == size);
        if (item != null)
            await js.SetStyleSheetAsync(item.Style, item.Url);
        await js.SetLocalStorageAsync(KeySize, size);
    }

    internal static Task<string> GetCurrentLanguageAsync(this JSService js) => js.GetLocalStorageAsync<string>(KeyLanguage);
    internal static Task SetCurrentLanguageAsync(this JSService js, string language) => js.SetLocalStorageAsync(KeyLanguage, language);

    /// <summary>
    /// 异步获取系统当前主题。
    /// </summary>
    /// <param name="js">JS服务。</param>
    /// <returns>当前主题。</returns>
    public static async Task<string> GetCurrentThemeAsync(this JSService js)
    {
        var theme = await js.GetLocalStorageAsync<string>(KeyTheme);
        if (string.IsNullOrWhiteSpace(theme))
        {
            var hour = DateTime.Now.Hour;
            theme = hour > 6 && hour < 20 ? "light" : "dark";
        }
        await js.SetThemeAsync(theme);
        return theme;
    }

    /// <summary>
    /// 异步存储系统当前主题。
    /// </summary>
    /// <param name="js">JS服务。</param>
    /// <param name="theme">当前主题。</param>
    /// <returns></returns>
    public static async Task SetCurrentThemeAsync(this JSService js, string theme)
    {
        await js.SetThemeAsync(theme);
        await js.SetLocalStorageAsync(KeyTheme, theme);
    }
}