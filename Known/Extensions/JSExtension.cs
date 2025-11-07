namespace Known.Extensions;

/// <summary>
/// JS运行时和服务扩展类。
/// </summary>
public static class JSExtension
{
    private static readonly string KeyUserInfo = "Known_User";
    private static readonly string KeyLocalInfo = "Known_LocalInfo";
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
    /// 异步注册通知回调方法。
    /// </summary>
    /// <typeparam name="T">调用组件类型。</typeparam>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="invoker">调用组件对象。</param>
    /// <param name="method">SignalR连接方法名。</param>
    /// <param name="invoke">调用组件的[JSInvokable]方法名。</param>
    /// <returns></returns>
    public static ValueTask RegisterNotifyAsync<T>(this IJSRuntime runtime, DotNetObjectReference<T> invoker, string method, string invoke) where T : class
    {
        return runtime.InvokeVoidAsync("KNotify.register", invoker, method, invoke);
    }

    /// <summary>
    /// 异步关闭通知事件。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="method">SignalR连接方法名。</param>
    /// <returns></returns>
    public static ValueTask CloseNotifyAsync(this IJSRuntime runtime, string method)
    {
        return runtime.InvokeVoidAsync("KNotify.close", method);
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

    /// <summary>
    /// 异步获取本地配置信息。
    /// </summary>
    /// <param name="js">JS服务。</param>
    /// <returns></returns>
    public static async Task<LocalInfo> GetLocalInfoAsync(this JSService js)
    {
        var info = await js.GetLocalStorageAsync<LocalInfo>(KeyLocalInfo);
        info ??= new LocalInfo { ClientId = $"KC-{Utils.GetGuid()}" };
        if (string.IsNullOrWhiteSpace(info.Theme))
        {
            var hour = DateTime.Now.Hour;
            info.Theme = hour > 6 && hour < 20 ? "light" : "dark";
        }
        return info;
    }

    internal static async Task SetLocalInfoAsync(this JSService js, LocalInfo info, UserSettingInfo setting = null, bool isLanguage = false)
    {
        if (info == null)
            return;

        if (!isLanguage)
        {
            if (setting != null)
            {
                info.Language = setting.Language;
                info.Theme = setting.Theme;
                info.Color = setting.ThemeColor;
                info.Size = setting.Size;
            }
            await js.InvokeVoidAsync("KBlazor.setLocalInfo", info);
        }
        await js.SetLocalStorageAsync(KeyLocalInfo, info);
    }
}