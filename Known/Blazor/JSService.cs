namespace Known.Blazor;

/// <summary>
/// JS服务类。
/// </summary>
[Service]
public partial class JSService
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly Lazy<Task<IJSObjectReference>> appTask;

    /// <summary>
    /// 构造函数，创建一个JS服务类的实例。
    /// </summary>
    /// <param name="jsRuntime">JS运行时对象。</param>
    public JSService(IJSRuntime jsRuntime)
    {
        var path = "./_content/Known/js/script.js?v=250522";
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", path).AsTask());
        if (!string.IsNullOrWhiteSpace(Config.App.JsPath))
            appTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", Config.App.JsPath).AsTask());
    }

    /// <summary>
    /// 异步调用项目JS方法，返回结果（即由Config.App.JsPath = "./script.js";设置的JS文件）。
    /// </summary>
    /// <typeparam name="T">JS执行结果返回类型。</typeparam>
    /// <param name="identifier">JS方法标识。</param>
    /// <param name="args">JS方法参数。</param>
    /// <returns>指定泛型的对象。</returns>
    public async Task<T> InvokeAppAsync<T>(string identifier, params object[] args)
    {
        try
        {
            var module = await appTask.Value;
            return await module.InvokeAsync<T>(identifier, args);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 异步调用项目JS方法，无返回结果（即由Config.App.JsPath = "./script.js";设置的JS文件）。
    /// </summary>
    /// <param name="identifier">JS方法标识。</param>
    /// <param name="args">JS方法参数。</param>
    /// <returns></returns>
    public async Task InvokeAppVoidAsync(string identifier, params object[] args)
    {
        try
        {
            var module = await appTask.Value;
            await module.InvokeVoidAsync(identifier, args);
        }
        catch
        {
        }
    }

    private async Task<T> InvokeAsync<T>(string identifier, params object[] args)
    {
        try
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<T>(identifier, args);
        }
        catch
        {
            return default;
        }
    }

    private async Task InvokeVoidAsync(string identifier, params object[] args)
    {
        try
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync(identifier, args);
        }
        catch
        {
        }
    }
}