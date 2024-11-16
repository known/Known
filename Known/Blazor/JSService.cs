using System.Diagnostics.CodeAnalysis;

namespace Known.Blazor;

/// <summary>
/// JS服务类。
/// </summary>
public class JSService
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly Lazy<Task<IJSObjectReference>> appTask;

    /// <summary>
    /// 构造函数，创建一个JS服务类的实例。
    /// </summary>
    /// <param name="jsRuntime">JS运行时对象。</param>
    public JSService(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Known/js/script.js?v=241016").AsTask());
        if (!string.IsNullOrWhiteSpace(Config.App.JsPath))
            appTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", Config.App.JsPath).AsTask());
    }

    #region Invoke
    /// <summary>
    /// 异步调用项目JS方法，返回结果（即由Config.App.JsPath = "./script.js";设置的JS文件）。
    /// </summary>
    /// <typeparam name="T">JS执行结果返回类型。</typeparam>
    /// <param name="identifier">JS方法标识。</param>
    /// <param name="args">JS方法参数。</param>
    /// <returns>指定泛型的对象。</returns>
    public async Task<T> InvokeAppAsync<T>(string identifier, params object[] args)
    {
        var module = await appTask.Value;
        return await module.InvokeAsync<T>(identifier, args);
    }

    /// <summary>
    /// 异步调用项目JS方法，无返回结果（即由Config.App.JsPath = "./script.js";设置的JS文件）。
    /// </summary>
    /// <param name="identifier">JS方法标识。</param>
    /// <param name="args">JS方法参数。</param>
    /// <returns></returns>
    public async Task InvokeAppVoidAsync(string identifier, params object[] args)
    {
        var module = await appTask.Value;
        await module.InvokeVoidAsync(identifier, args);
    }

    private async Task<T> InvokeAsync<T>(string identifier, params object[] args)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<T>(identifier, args);
    }

    private async Task InvokeVoidAsync(string identifier, params object[] args)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync(identifier, args);
    }
    #endregion

    #region Common
    /// <summary>
    /// 异步执行一段JS脚本，返回执行结果对象。
    /// </summary>
    /// <param name="script">JS脚本。</param>
    /// <returns>执行结果对象。</returns>
    public Task<object> RunAsync(string script) => InvokeAsync<object>("KBlazor.runScript", script);

    /// <summary>
    /// 异步执行一段JS脚本，无返回结果。
    /// </summary>
    /// <param name="script">JS脚本。</param>
    /// <returns></returns>
    public Task RunVoidAsync(string script) => InvokeVoidAsync("KBlazor.runScriptVoid", script);

    /// <summary>
    /// 异步单击前端指定ID的控件。
    /// </summary>
    /// <param name="clientId">前端控件ID。</param>
    /// <returns></returns>
    public Task ClickAsync(string clientId) => InvokeVoidAsync("KBlazor.elemClick", clientId);

    /// <summary>
    /// 异步将前端控件设为是否可用。
    /// </summary>
    /// <param name="clientId">前端控件ID。</param>
    /// <param name="enabled">是否可用。</param>
    /// <returns></returns>
    public Task EnabledAsync(string clientId, bool enabled) => InvokeVoidAsync("KBlazor.elemEnabled", clientId, enabled);
    
    /// <summary>
    /// 高亮显示代码。
    /// </summary>
    /// <param name="code">代码。</param>
    /// <param name="language">代码语言。</param>
    /// <returns>高亮代码。</returns>
    public Task<string> HighlightAsync(string code, string language) => InvokeAsync<string>("KBlazor.highlight", code, language);
    #endregion

    #region Storage
    /// <summary>
    /// 异步获取浏览器加密存储的泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型对象类型。</typeparam>
    /// <param name="key">对象存储键。</param>
    /// <returns>泛型对象。</returns>
    public async Task<T> GetLocalStorageAsync<T>(string key)
    {
        var value = await InvokeAsync<string>("KBlazor.getLocalStorage", key);
        if (string.IsNullOrWhiteSpace(value))
            return default;

        try
        {
            var json = DecryptString(value);
            return Utils.FromJson<T>(json);
        }
        catch
        {
            try
            {
                var data = Utils.FromJson<T>(value);
                await SetLocalStorageAsync(key, data);
                return data;
            }
            catch
            {
                return default;
            }
        }
    }

    /// <summary>
    /// 异步设置浏览器加密存储对象。
    /// </summary>
    /// <param name="key">对象存储键。</param>
    /// <param name="data">对象数据。</param>
    /// <returns></returns>
    public async Task SetLocalStorageAsync(string key, object data)
    {
        var value = EncryptString(data);
        await InvokeVoidAsync("KBlazor.setLocalStorage", key, value);
    }

    /// <summary>
    /// 异步设置样式文件。
    /// </summary>
    /// <param name="match">原匹配样式文件。</param>
    /// <param name="href">新样式文件。</param>
    /// <returns></returns>
    public Task SetStyleSheetAsync(string match, string href) => InvokeVoidAsync("KBlazor.setStyleSheet", match, href);

    /// <summary>
    /// 异步插入样式文件。
    /// </summary>
    /// <param name="match">原匹配样式文件。</param>
    /// <param name="href">新样式文件。</param>
    /// <returns></returns>
    public Task InsertStyleSheetAsync(string match, string href) => InvokeVoidAsync("KBlazor.insertStyleSheet", match, href);

    /// <summary>
    /// 异步添加样式文件。
    /// </summary>
    /// <param name="href">样式文件。</param>
    /// <returns></returns>
    public Task AddStyleSheetAsync(string href) => InvokeVoidAsync("KBlazor.addStyleSheet", href);

    /// <summary>
    /// 异步删除样式文件。
    /// </summary>
    /// <param name="href">样式文件。</param>
    /// <returns></returns>
    public Task RemoveStyleSheetAsync(string href) => InvokeVoidAsync("KBlazor.removeStyleSheet", href);

    internal Task SetThemeAsync(string theme) => InvokeVoidAsync("KBlazor.setTheme", theme);

    /// <summary>
    /// 异步获取浏览器加密会话存储的泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型对象类型。</typeparam>
    /// <param name="key">对象存储键。</param>
    /// <returns>泛型对象。</returns>
    public async Task<T> GetSessionStorageAsync<T>(string key)
    {
        var value = await InvokeAsync<string>("KBlazor.getSessionStorage", key);
        if (string.IsNullOrWhiteSpace(value))
            return default;

        var json = DecryptString(value);
        return Utils.FromJson<T>(json);
    }

    /// <summary>
    /// 异步设置浏览器加密会话存储对象。
    /// </summary>
    /// <param name="key">对象存储键。</param>
    /// <param name="data">对象数据。</param>
    /// <returns></returns>
    public async Task SetSessionStorageAsync(string key, object data)
    {
        var value = EncryptString(data);
        await InvokeVoidAsync("KBlazor.setSessionStorage", key, value);
    }

    private static string EncryptString(object value)
    {
        if (value == null)
            return null;

        var json = Utils.ToJson(value);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    private static string DecryptString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var bytes = Convert.FromBase64String(value);
        return Encoding.UTF8.GetString(bytes);
    }
    #endregion

    #region Screen
    /// <summary>
    /// 异步全屏显示系统。
    /// </summary>
    /// <returns></returns>
    public Task OpenFullScreenAsync() => InvokeVoidAsync("KBlazor.openFullScreen");

    /// <summary>
    /// 异步关闭全屏显示。
    /// </summary>
    /// <returns></returns>
    public Task CloseFullScreenAsync() => InvokeVoidAsync("KBlazor.closeFullScreen");
    #endregion

    #region Print
    /// <summary>
    /// 异步调用浏览器打印组件内容。
    /// </summary>
    /// <typeparam name="T">组件类型。</typeparam>
    /// <param name="action">组件内容操作方法。</param>
    /// <returns></returns>
    public async Task PrintAsync<T>(Action<IPrintRenderer<T>> action) where T : Microsoft.AspNetCore.Components.IComponent
    {
        var services = new ServiceCollection();
        services.AddScoped<IJSRuntime, PrintJSRuntime>();
        services.AddScoped<JSService>();
        //services.AddHttpContextAccessor();
        var provider = services.BuildServiceProvider();
        var component = new ComponentRenderer<T>().AddServiceProvider(provider);
        action?.Invoke(component);
        var content = component.Render();
        await PrintAsync(content);
    }

    /// <summary>
    /// 异步调用浏览器打印HTML内容。
    /// </summary>
    /// <param name="content">HTML内容。</param>
    /// <returns></returns>
    public Task PrintAsync(string content) => InvokeVoidAsync("KBlazor.printContent", content);
    #endregion

    #region Download
    /// <summary>
    /// 异步下载文件。
    /// </summary>
    /// <param name="fileName">文件名。</param>
    /// <param name="url">下载地址。</param>
    /// <returns></returns>
    public Task DownloadFileAsync(string fileName, string url) => InvokeVoidAsync("KBlazor.downloadFileByUrl", fileName, url);

    /// <summary>
    /// 异步下载文件流。
    /// </summary>
    /// <param name="fileName">文件名。</param>
    /// <param name="stream">文件流。</param>
    /// <returns></returns>
    public async Task DownloadFileAsync(string fileName, Stream stream)
    {
        var module = await moduleTask.Value;
        using var streamRef = new DotNetStreamReference(stream);
        await module.InvokeVoidAsync("KBlazor.downloadFileByStream", fileName, streamRef);
    }
    #endregion

    #region Pdf
    /// <summary>
    /// 异步显示PDF文件。
    /// </summary>
    /// <param name="id">PDF前端控件ID。</param>
    /// <param name="stream">PDF文件流。</param>
    /// <returns></returns>
    public async Task ShowPdfAsync(string id, Stream stream)
    {
        if (stream == null)
            return;

        var module = await moduleTask.Value;
        using var streamRef = new DotNetStreamReference(stream);
        await module.InvokeVoidAsync("KBlazor.showPdf", id, streamRef);
    }
    #endregion

    #region Image
    /// <summary>
    /// 异步预览图片。
    /// </summary>
    /// <param name="inputElem">图片附件上传组件实例。</param>
    /// <param name="imgElem">图片预览Img控件实例。</param>
    /// <returns></returns>
    public Task PreviewImageAsync(ElementReference? inputElem, ElementReference imgElem) => InvokeVoidAsync("KBlazor.previewImage", inputElem, imgElem);

    /// <summary>
    /// 根据Img控件ID预览图片。
    /// </summary>
    /// <param name="inputElem">图片附件上传组件实例。</param>
    /// <param name="imgId">图片前端Img控件ID。</param>
    /// <returns></returns>
    public Task PreviewImageByIdAsync(ElementReference? inputElem, string imgId) => InvokeVoidAsync("KBlazor.previewImageById", inputElem, imgId);
    
    /// <summary>
    /// 异步绘制验证码组件。
    /// </summary>
    /// <param name="id">验证码控件ID。</param>
    /// <param name="code">验证码字符串。</param>
    /// <returns></returns>
    public Task CaptchaAsync(string id, string code) => InvokeVoidAsync("KBlazor.captcha", id, code);
    #endregion

    #region Chart
    internal Task ShowChartAsync(string id, object option) => InvokeVoidAsync("KBlazor.showChart", id, option);
    internal Task ShowBarcodeAsync(string id, string value, object option) => InvokeVoidAsync("KBlazor.showBarcode", id, value, option);
    internal Task ShowQRCodeAsync(string id, object option) => InvokeVoidAsync("KBlazor.showQRCode", id, option);
    #endregion
}

class PrintJSRuntime : IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object[] args)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object[] args)
    {
        throw new NotImplementedException();
    }
}