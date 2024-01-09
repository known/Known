using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Known.Blazor;

public class JSService
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly Lazy<Task<IJSObjectReference>> appTask;

    public JSService(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Known/script.js").AsTask());
        if (!string.IsNullOrWhiteSpace(Config.App.JsPath))
            appTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", Config.App.JsPath).AsTask());
    }

    #region Invoke
    public async Task<T> InvokeAppAsync<T>(string identifier, params object[] args)
    {
        var module = await appTask.Value;
        return await module.InvokeAsync<T>(identifier, args);
    }

    public async void InvokeAppVoidAsync(string identifier, params object[] args)
    {
        var module = await appTask.Value;
        await module.InvokeVoidAsync(identifier, args);
    }

    private async Task<T> InvokeAsync<T>(string identifier, params object[] args)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<T>(identifier, args);
    }

    private async void InvokeVoidAsync(string identifier, params object[] args)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync(identifier, args);
    }
    #endregion

    #region Common
    public void Click(string clientId) => InvokeVoidAsync("KBlazor.elemClick", clientId);
    public void Enabled(string clientId, bool enabled) => InvokeVoidAsync("KBlazor.elemEnabled", clientId, enabled);
    internal Task<string> HighlightAsync(string code, string language) => InvokeAsync<string>("KBlazor.highlight", code, language);
    #endregion

    #region LocalStorage
    public async Task<T> GetLocalStorage<T>(string key)
    {
        var value = await InvokeAsync<string>("KBlazor.getLocalStorage", key);
        return Utils.FromJson<T>(value);
    }

    public void SetLocalStorage(string key, object value) => InvokeVoidAsync("KBlazor.setLocalStorage", key, value);

    private readonly string KeyLanguage = "Known_Language";
    internal Task<string> GetCurrentLanguage() => GetLocalStorage<string>(KeyLanguage);
    public void SetCurrentLanguage(string language) => SetLocalStorage(KeyLanguage, language);

    private readonly string KeyTheme = "Known_Theme";
    public async Task<string> GetCurrentTheme()
    {
        var theme = await GetLocalStorage<string>(KeyTheme);
        if (string.IsNullOrWhiteSpace(theme))
        {
            var hour = DateTime.Now.Hour;
            theme = hour > 6 && hour < 20 ? "klight" : "dark";
        }
        return theme;
    }
    public void SetCurrentTheme(string theme) => SetLocalStorage(KeyTheme, theme);

    private readonly string KeyLoginInfo = "Known_LoginInfo";
    internal Task<T> GetLoginInfo<T>() => GetLocalStorage<T>(KeyLoginInfo);
    internal void SetLoginInfo(object value) => SetLocalStorage(KeyLoginInfo, value);
    #endregion

    #region Screen
    public void OpenFullScreen() => InvokeVoidAsync("KBlazor.openFullScreen");
    public void CloseFullScreen() => InvokeVoidAsync("KBlazor.closeFullScreen");
    #endregion

    #region Print
    public void Print<T>(Action<ComponentRenderer<T>> action) where T : IComponent
    {
        var services = new ServiceCollection();
        services.AddScoped<IJSRuntime, PrintJSRuntime>();
        services.AddScoped<JSService>();
        services.AddHttpContextAccessor();
        var provider = services.BuildServiceProvider();
        var component = new ComponentRenderer<T>().AddServiceProvider(provider);
        action?.Invoke(component);
        var content = component.Render();
        Print(content);
    }

    public void Print(string content) => InvokeVoidAsync("KBlazor.printContent", content);
    #endregion

    #region Download
    public void DownloadFile(string fileName, string url) => InvokeVoidAsync("KBlazor.downloadFileByUrl", fileName, url);

    public async void DownloadFile(string fileName, Stream stream)
    {
        var module = await moduleTask.Value;
        using var streamRef = new DotNetStreamReference(stream);
        await module.InvokeVoidAsync("KBlazor.downloadFileByStream", fileName, streamRef);
    }
    #endregion

    #region Pdf
    public async void ShowPdf(string id, Stream stream)
    {
        if (stream == null)
            return;

        var module = await moduleTask.Value;
        using var streamRef = new DotNetStreamReference(stream);
        await module.InvokeVoidAsync("KBlazor.showPdf", id, streamRef);
    }
    #endregion

    #region Image
    public void Captcha(string id, string code) => InvokeVoidAsync("KBlazor.captcha", id, code);
    internal void ShowBarcode(string id, string value, object option) => InvokeVoidAsync("KBlazor.showBarcode", id, value, option);
    internal void ShowQRCode(string id, object option) => InvokeVoidAsync("KBlazor.showQRCode", id, option);
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