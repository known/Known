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
    /// 安全调用JS互操作。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="method">JS方法。</param>
    /// <param name="args">JS方法参数。</param>
    /// <returns></returns>
    public static async Task InvokeJsAsync(this IJSRuntime runtime, string method, params object[] args)
    {
        try
        {
            await runtime.InvokeVoidAsync(method, args);
        }
        catch (JSDisconnectedException)
        {
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// 安全调用JS互操作。
    /// </summary>
    /// <typeparam name="T">返回结果类型。</typeparam>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="method">JS方法。</param>
    /// <param name="args">JS方法参数。</param>
    /// <returns></returns>
    public static async Task<T> InvokeJsAsync<T>(this IJSRuntime runtime, string method, params object[] args)
    {
        try
        {
            var value = await runtime.InvokeAsync<T>(method, args);
            return value;
        }
        catch (JSDisconnectedException)
        {
            return default;
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// 异步检查是否是移动端访问。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <returns></returns>
    public static Task<bool> CheckMobileAsync(this IJSRuntime runtime)
    {
        return runtime.InvokeJsAsync<bool>("KUtils.isMobile");
    }

    /// <summary>
    /// 异步高亮显示页面代码。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <returns></returns>
    public static Task HighlightAllAsync(this IJSRuntime runtime)
    {
        return runtime.InvokeJsAsync("Prism.highlightAll");
    }

    /// <summary>
    /// 异步执行一段JS脚本，返回执行结果对象。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="script">JS脚本。</param>
    /// <returns>执行结果对象。</returns>
    public static Task<object> RunAsync(this IJSRuntime runtime, string script)
    {
        return runtime.InvokeJsAsync<object>("KUtils.runScript", script);
    }

    /// <summary>
    /// 异步执行一段JS脚本，无返回结果。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="script">JS脚本。</param>
    /// <returns></returns>
    public static Task RunVoidAsync(this IJSRuntime runtime, string script)
    {
        return runtime.InvokeJsAsync("KUtils.runScriptVoid", script);
    }

    /// <summary>
    /// 异步单击前端指定ID的控件。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="clientId">前端控件ID。</param>
    /// <returns></returns>
    public static Task ClickAsync(this IJSRuntime runtime, string clientId)
    {
        return runtime.InvokeJsAsync("KUtils.elemClick", clientId);
    }

    /// <summary>
    /// 异步将前端控件设为是否可用。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="clientId">前端控件ID。</param>
    /// <param name="enabled">是否可用。</param>
    /// <returns></returns>
    public static Task EnabledAsync(this IJSRuntime runtime, string clientId, bool enabled)
    {
        return runtime.InvokeJsAsync("KUtils.elemEnabled", clientId, enabled);
    }

    /// <summary>
    /// 异步将滚动条自动平滑滚动到顶部。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="elemId">滚动元素ID。</param>
    /// <returns></returns>
    public static Task ScrollToTopAsync(this IJSRuntime runtime, string elemId)
    {
        return runtime.InvokeJsAsync("KUtils.scrollToTop", elemId);
    }

    /// <summary>
    /// 异步将滚动条自动平滑滚动到底部。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="elemId">滚动元素ID。</param>
    /// <returns></returns>
    public static Task ScrollToBottomAsync(this IJSRuntime runtime, string elemId)
    {
        return runtime.InvokeJsAsync("KUtils.scrollToBottom", elemId);
    }

    /// <summary>
    /// 异步复制文本到剪贴板。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="text">要复制的文本。</param>
    /// <returns></returns>
    public static Task CopyTextAsync(this IJSRuntime runtime, string text)
    {
        return runtime.InvokeJsAsync("navigator.clipboard.writeText", text);
    }

    /// <summary>
    /// 异步粘贴剪贴板里的数据。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="action">粘贴数据处理委托。</param>
    /// <returns></returns>
    public static async Task PasteTextAsync(this IJSRuntime runtime, Action<string> action)
    {
        var text = await runtime.InvokeJsAsync<string>("navigator.clipboard.readText");
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
    public static Task RegisterNotifyAsync<T>(this IJSRuntime runtime, DotNetObjectReference<T> invoker, string method, string invoke) where T : class
    {
        return runtime.InvokeJsAsync("KNotify.register", invoker, method, invoke);
    }

    /// <summary>
    /// 异步注册快捷键。
    /// </summary>
    /// <typeparam name="T">调用组件类型。</typeparam>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="owner">快捷键归属标识。</param>
    /// <param name="invoker">调用组件对象。</param>
    /// <param name="hotkey">快捷键。</param>
    /// <param name="invoke">调用组件的[JSInvokable]方法名。</param>
    /// <returns></returns>
    public static Task RegisterHotkeyAsync<T>(this IJSRuntime runtime, string owner, DotNetObjectReference<T> invoker, string hotkey, string invoke) where T : class
    {
        return runtime.InvokeJsAsync("KHotkey.register", owner, invoker, new HotkeyInfo(hotkey, invoke));
    }

    /// <summary>
    /// 异步注册多个快捷键。
    /// </summary>
    /// <typeparam name="T">调用组件类型。</typeparam>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="owner">快捷键归属标识。</param>
    /// <param name="invoker">调用组件对象。</param>
    /// <param name="hotkeys">快捷键集合。</param>
    /// <returns></returns>
    public static Task RegisterHotkeysAsync<T>(this IJSRuntime runtime, string owner, DotNetObjectReference<T> invoker, params HotkeyInfo[] hotkeys) where T : class
    {
        return runtime.InvokeJsAsync("KHotkey.register", owner, invoker, hotkeys);
    }

    /// <summary>
    /// 异步释放快捷键。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="owner">快捷键归属标识。</param>
    /// <param name="hotkey">快捷键。</param>
    /// <returns></returns>
    public static Task DisposeHotkeyAsync(this IJSRuntime runtime, string owner, string hotkey)
    {
        return runtime.InvokeJsAsync("KHotkey.dispose", owner, new[] { hotkey });
    }

    /// <summary>
    /// 异步释放快捷键。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="owner">快捷键归属标识。</param>
    /// <param name="hotkeys">快捷键集合，为空时释放当前归属下全部快捷键。</param>
    /// <returns></returns>
    public static Task DisposeHotkeysAsync(this IJSRuntime runtime, string owner, params string[] hotkeys)
    {
        return runtime.InvokeJsAsync("KHotkey.dispose", owner, hotkeys);
    }

    /// <summary>
    /// 异步关闭通知事件。
    /// </summary>
    /// <param name="runtime">JS运行时。</param>
    /// <param name="method">SignalR连接方法名。</param>
    /// <returns></returns>
    public static Task CloseNotifyAsync(this IJSRuntime runtime, string method)
    {
        return runtime.InvokeJsAsync("KNotify.close", method);
    }

    internal static Task<string> GetUserAgentAsync(this IJSRuntime runtime)
    {
        return runtime.InvokeJsAsync<string>("KUtils.getUserAgent");
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

    internal static async Task SetLocalInfoAsync(this JSService js, LocalInfo info)
    {
        if (info == null)
            return;

        await js.InvokeAsync("KBlazor.setLocalInfo", info);
        await js.SetLocalStorageAsync(KeyLocalInfo, info);
    }
}