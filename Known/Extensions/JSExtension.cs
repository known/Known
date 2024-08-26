namespace Known.Extensions;

public static class JSExtension
{
    private static readonly string KeyUserInfo = "Known_User";
    private static readonly string KeyLoginInfo = "Known_LoginInfo";
    private static readonly string KeySize = "Known_Size";
    private static readonly string KeyLanguage = "Known_Language";
    private static readonly string KeyTheme = "Known_Theme";

    public static async Task PasteTextAsync(this IJSRuntime js, Action<string> action)
    {
        var text = await js.InvokeAsync<string>("navigator.clipboard.readText", null);
        action?.Invoke(text);
    }

    public static Task<UserInfo> GetUserInfoAsync(this JSService js) => js.GetSessionStorageAsync<UserInfo>(KeyUserInfo);
    public static Task SetUserInfoAsync(this JSService js, object data) => js.SetSessionStorageAsync(KeyUserInfo, data);

    internal static Task<T> GetLoginInfoAsync<T>(this JSService js) => js.GetLocalStorageAsync<T>(KeyLoginInfo);
    internal static Task SetLoginInfoAsync(this JSService js, object value) => js.SetLocalStorageAsync(KeyLoginInfo, value);

    internal static Task<string> GetCurrentSizeAsync(this JSService js) => js.GetLocalStorageAsync<string>(KeySize);
    internal static async Task SetCurrentSizeAsync(this JSService js, string size)
    {
        var item = UIConfig.Sizes.FirstOrDefault(s => s.Id == size);
        if (item != null)
            await js.SetStyleAsync(item.Style, item.Url);
        await js.SetLocalStorageAsync(KeySize, size);
    }

    internal static Task<string> GetCurrentLanguageAsync(this JSService js) => js.GetLocalStorageAsync<string>(KeyLanguage);
    internal static Task SetCurrentLanguageAsync(this JSService js, string language) => js.SetLocalStorageAsync(KeyLanguage, language);

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
    public static async Task SetCurrentThemeAsync(this JSService js, string theme)
    {
        await js.SetThemeAsync(theme);
        await js.SetLocalStorageAsync(KeyTheme, theme);
    }
}