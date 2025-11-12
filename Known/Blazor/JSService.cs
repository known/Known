namespace Known.Blazor;

/// <summary>
/// JS服务类。
/// </summary>
public partial class JSService
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly UIService ui;

    /// <summary>
    /// 构造函数，创建一个JS服务类的实例。
    /// </summary>
    /// <param name="ui">UI服务对象。</param>
    /// <param name="jsRuntime">JS运行时对象。</param>
    public JSService(UIService ui, IJSRuntime jsRuntime)
    {
        this.ui = ui;
        var path = "./_content/Known/js/script.js?v=20251027";
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", path).AsTask());
    }

    internal async Task<T> InvokeAsync<T>(string method, params object[] args)
    {
        try
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<T>(method, args);
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

    internal async Task InvokeAsync(string method, params object[] args)
    {
        try
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync(method, args);
        }
        catch (JSDisconnectedException)
        {
        }
        catch (Exception)
        {
        }
    }
}