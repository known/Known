﻿namespace Known.Blazor;

public class JSService
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly Lazy<Task<IJSObjectReference>> appTask;

    public JSService(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Known/script.js?v=2408262213").AsTask());
        if (!string.IsNullOrWhiteSpace(Config.App.JsPath))
            appTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", Config.App.JsPath).AsTask());
    }

    #region Invoke
    public async Task<T> InvokeAppAsync<T>(string identifier, params object[] args)
    {
        var module = await appTask.Value;
        return await module.InvokeAsync<T>(identifier, args);
    }

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
    public Task<object> RunAsync(string script) => InvokeAsync<object>("KBlazor.runScript", script);
    public Task RunVoidAsync(string script) => InvokeVoidAsync("KBlazor.runScriptVoid", script);
    public Task ClickAsync(string clientId) => InvokeVoidAsync("KBlazor.elemClick", clientId);
    public Task EnabledAsync(string clientId, bool enabled) => InvokeVoidAsync("KBlazor.elemEnabled", clientId, enabled);
    internal Task<string> HighlightAsync(string code, string language) => InvokeAsync<string>("KBlazor.highlight", code, language);
    #endregion

    #region Storage
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

    public async Task SetLocalStorageAsync(string key, object data)
    {
        var value = EncryptString(data);
        await InvokeVoidAsync("KBlazor.setLocalStorage", key, value);
    }

    internal Task SetStyleAsync(string match, string href) => InvokeVoidAsync("KBlazor.setStyle", match, href);
    internal Task SetThemeAsync(string theme) => InvokeVoidAsync("KBlazor.setTheme", theme);

    public async Task<T> GetSessionStorageAsync<T>(string key)
    {
        var value = await InvokeAsync<string>("KBlazor.getSessionStorage", key);
        if (string.IsNullOrWhiteSpace(value))
            return default;

        var json = DecryptString(value);
        return Utils.FromJson<T>(json);
    }

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
    public Task OpenFullScreenAsync() => InvokeVoidAsync("KBlazor.openFullScreen");
    public Task CloseFullScreenAsync() => InvokeVoidAsync("KBlazor.closeFullScreen");
    #endregion

    #region Print
    public async Task PrintAsync<T>(Action<ComponentRenderer<T>> action) where T : Microsoft.AspNetCore.Components.IComponent
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

    public Task PrintAsync(string content) => InvokeVoidAsync("KBlazor.printContent", content);
    #endregion

    #region Download
    public Task DownloadFileAsync(string fileName, string url) => InvokeVoidAsync("KBlazor.downloadFileByUrl", fileName, url);

    public async Task DownloadFileAsync(string fileName, Stream stream)
    {
        var module = await moduleTask.Value;
        using var streamRef = new DotNetStreamReference(stream);
        await module.InvokeVoidAsync("KBlazor.downloadFileByStream", fileName, streamRef);
    }
    #endregion

    #region Pdf
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
    public Task PreviewImageAsync(ElementReference? inputElem, ElementReference imgElem) => InvokeVoidAsync("KBlazor.previewImage", inputElem, imgElem);
    public Task PreviewImageByIdAsync(ElementReference? inputElem, string imgId) => InvokeVoidAsync("KBlazor.previewImageById", inputElem, imgId);
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