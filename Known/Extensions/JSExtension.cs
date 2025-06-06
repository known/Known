﻿namespace Known.Extensions;

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
    /// 判断是否是Server运行模式。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <returns></returns>
    public static bool IsServerMode(this IJSRuntime runtime)
    {
        return runtime.GetType().ToString() == "Microsoft.AspNetCore.Components.Server.Circuits.RemoteJSRuntime";
    }

    /// <summary>
    /// 异步检查是否是移动端访问。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <returns></returns>
    public static ValueTask<bool> CheckMobileAsync(this IJSRuntime runtime)
    {
        return runtime.InvokeAsync<bool>("isMobile");
    }

    /// <summary>
    /// 异步高亮显示页面代码。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <returns></returns>
    public static ValueTask HighlightAllAsync(this IJSRuntime runtime)
    {
        return runtime.InvokeVoidAsync("Prism.highlightAll");
    }

    /// <summary>
    /// 异步复制文本到剪贴板。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="text">要复制的文本。</param>
    /// <returns></returns>
    public static ValueTask CopyTextAsync(this IJSRuntime runtime, string text)
    {
        return runtime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }

    /// <summary>
    /// 异步粘贴剪贴板里的数据。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="action">粘贴数据处理委托。</param>
    /// <returns></returns>
    public static async Task PasteTextAsync(this IJSRuntime runtime, Action<string> action)
    {
        var text = await runtime.InvokeAsync<string>("navigator.clipboard.readText", null);
        action?.Invoke(text);
    }

    /// <summary>
    /// 异步下载文件。
    /// </summary>
    /// <param name="js">JS服务。</param>
    /// <param name="info">文件信息。</param>
    /// <returns></returns>
    public static async Task DownloadFileAsync(this JSService js, FileDataInfo info)
    {
        if (info != null && info.Bytes != null && info.Bytes.Length > 0)
        {
            var stream = new MemoryStream(info.Bytes);
            await js.DownloadFileAsync(info.Name, stream);
        }
    }

    /// <summary>
    /// 异步下载文件。
    /// </summary>
    /// <param name="js">JS服务。</param>
    /// <param name="fileName">文件名。</param>
    /// <param name="bytes">文件字节。</param>
    /// <returns></returns>
    public static Task DownloadFileAsync(this JSService js, string fileName, byte[] bytes)
    {
        var info = new FileDataInfo(fileName, bytes);
        return js.DownloadFileAsync(info);
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