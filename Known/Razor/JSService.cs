using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Known.Razor;

public class JSService
{
    private readonly IJSRuntime jsRuntime;
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly Lazy<Task<IJSObjectReference>> appTask;

    public JSService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Known/script.js").AsTask());
        if (!string.IsNullOrWhiteSpace(Config.AppJsPath))
            appTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", Config.AppJsPath).AsTask());
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

    #region LocalStorage
    public async Task<T> GetLocalStorage<T>(string key)
    {
        var value = await InvokeAsync<string>("KRazor.getLocalStorage", key);
        return Utils.FromJson<T>(value);
    }

    public void SetLocalStorage(string key, object value) => InvokeVoidAsync("KRazor.setLocalStorage", key, value);
    #endregion

    #region Screen
    public void OpenFullScreen() => InvokeVoidAsync("KRazor.openFullScreen");
    public void CloseFullScreen() => InvokeVoidAsync("KRazor.closeFullScreen");
    #endregion

    #region Print
    public void Print<T>(Action<ComponentRenderer<T>> action) where T : IComponent
    {
        var services = new ServiceCollection();
        services.AddScoped<IJSRuntime, ComJSRuntime>();
        //services.AddScoped<UIService>();
        var provider = services.BuildServiceProvider();
        var component = new ComponentRenderer<T>().AddServiceProvider(provider);
        action?.Invoke(component);
        var content = component.Render();
        Print(content);
    }

    public void Print(string content) => InvokeVoidAsync("KRazor.printContent", content);
    #endregion
}

class ComJSRuntime : IJSRuntime
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