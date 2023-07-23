namespace Known.Razor;

public partial class UIService
{
    private readonly IJSRuntime jsRuntime;
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public UIService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Known.Razor/script.js").AsTask());
    }

    internal void InitMenu() => InvokeVoidAsync("KRazor.initMenu");
    internal void Layout(string id) => InvokeVoidAsync("KRazor.layout", id);
    public void AppendBody(string html) => InvokeVoidAsync("KRazor.appendBody", html);
    public void ShowFrame(string id, string url) => InvokeVoidAsync("KRazor.showFrame", id, url);
    public void ShowQuickView(string id) => ToggleClass(id, "active");
    public void ShowLoading() => InvokeVoidAsync("KRazor.showLoading");
    public void HideLoading() => InvokeVoidAsync("KRazor.hideLoading");
    public void OpenFullScreen() => InvokeVoidAsync("KRazor.openFullScreen");
    public void CloseFullScreen() => InvokeVoidAsync("KRazor.closeFullScreen");
    public void OpenLink(string url) => InvokeVoidAsync("KRazor.openLink", url);
    public void Click(string clientId) => InvokeVoidAsync("KRazor.elemClick", clientId);
    public void Enabled(string clientId, bool enabled) => InvokeVoidAsync("KRazor.elemEnabled", clientId, enabled);
    public void ToggleClass(string clientId, string className) => InvokeVoidAsync("KRazor.toggleClass", clientId, className);
    public void CopyToClipboard(string text) => InvokeVoidAsync("KRazor.copyToClipboard", text);
    public async void Back() => await jsRuntime.InvokeAsync<string>("history.go", -1);

    public async Task<T> InvokeAsync<T>(string identifier, params object[] args)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<T>(identifier, args);
    }

    private async void InvokeVoidAsync(string identifier, params object[] args)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync(identifier, args);
    }
}