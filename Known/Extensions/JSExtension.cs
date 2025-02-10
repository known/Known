namespace Known.Extensions;

/// <summary>
/// JS运行时和服务扩展类。
/// </summary>
public static class JSExtension
{
    private static readonly string KeyUserInfo = "Known_User";
    private static readonly string KeySize = "Known_Size";
    private static readonly string KeyLanguage = "Known_Language";
    private static readonly string KeyLoginInfo = "Known_LoginInfo";

    /// <summary>
    /// 异步检查是否是移动端访问。
    /// </summary>
    /// <param name="js">JS运行时。</param>
    /// <returns></returns>
    public static ValueTask<bool> CheckMobileAsync(this IJSRuntime js)
    {
        return js.InvokeAsync<bool>("isMobile");
    }

    /// <summary>
    /// 异步高亮显示页面代码。
    /// </summary>
    /// <param name="js">JS运行时。</param>
    /// <returns></returns>
    public static ValueTask HighlightAllAsync(this IJSRuntime js)
    {
        return js.InvokeVoidAsync("Prism.highlightAll");
    }

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
    /// 异步自动填充页面高度。
    /// </summary>
    /// <param name="js">JS运行时。</param>
    /// <param name="isResize">是否重设大小。</param>
    /// <returns></returns>
    public static ValueTask FillHeightAsync(this IJSRuntime js, bool isResize = false)
    {
        return js.InvokeVoidAsync("K_AutoFillHeight", isResize);
    }

    /// <summary>
    /// 异步获取浏览器会话存储的当前用户信息。
    /// </summary>
    /// <param name="js">JS服务。</param>
    /// <returns>当前用户信息。</returns>
    public static Task<UserInfo> GetUserInfoAsync(this JSService js)
    {
        return js.GetSessionStorageAsync<UserInfo>(KeyUserInfo);
    }

    /// <summary>
    /// 异步存储当前用户信息到浏览器会话中。
    /// </summary>
    /// <param name="js">JS服务。</param>
    /// <param name="data">当前用户信息。</param>
    /// <returns></returns>
    public static Task SetUserInfoAsync(this JSService js, object data)
    {
        return js.SetSessionStorageAsync(KeyUserInfo, data);
    }

    internal static Task<T> GetLoginInfoAsync<T>(this JSService js)
    {
        return js.GetLocalStorageAsync<T>(KeyLoginInfo);
    }

    internal static Task SetLoginInfoAsync(this JSService js, object value)
    {
        return js.SetLocalStorageAsync(KeyLoginInfo, value);
    }

    internal static async Task SetCurrentSizeAsync(this JSService js, string size)
    {
        var item = UIConfig.Sizes.FirstOrDefault(s => s.Id == size);
        if (item != null)
            await js.SetStyleSheetAsync(item.Style, item.Url);
        await js.SetLocalStorageAsync(KeySize, size);
    }

    internal static Task<string> GetCurrentLanguageAsync(this JSService js)
    {
        return js.GetLocalStorageAsync<string>(KeyLanguage);
    }

    internal static Task SetCurrentLanguageAsync(this JSService js, string language)
    {
        return js.SetLocalStorageAsync(KeyLanguage, language);
    }
}