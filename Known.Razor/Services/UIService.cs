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

    [JSInvokable]
    public static Task<string> Hello(string name) => Task.FromResult($"Hello {name}");
    public void AppendBody(string html) => InvokeVoidAsync("KR_appendBody", html);
    public void ShowFrame(string id, string url) => InvokeVoidAsync("KR_showFrame", id, url);
    public void ShowLoading() => InvokeVoidAsync("KR_showLoading");
    public void HideLoading() => InvokeVoidAsync("KR_hideLoading");
    public void OpenFullScreen() => InvokeVoidAsync("KR_openFullScreen");
    public void CloseFullScreen() => InvokeVoidAsync("KR_closeFullScreen");
    public void OpenLink(string url) => InvokeVoidAsync("KR_openLink", url);
    public void Click(string clientId) => InvokeVoidAsync("KR_elemClick", clientId);
    public void Enabled(string clientId, bool enabled) => InvokeVoidAsync("KR_elemEnabled", clientId, enabled);
    public void ToggleClass(string clientId, string className) => InvokeVoidAsync("KR_toggleClass", clientId, className);
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