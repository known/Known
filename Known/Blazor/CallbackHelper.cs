namespace Known.Blazor;

/// <summary>
/// JS回调C#方法帮助者类。
/// </summary>
public sealed class CallbackHelper
{
    private static readonly ConcurrentDictionary<string, Dictionary<string, Delegate>> actions = new();

    private CallbackHelper() { }

    /// <summary>
    /// 注册C#方法。
    /// </summary>
    /// <param name="id">方法ID。</param>
    /// <param name="key">方法Key。</param>
    /// <param name="action">方法委托。</param>
    public static void Register(string id, string key, Delegate action)
    {
        var list = actions.GetOrAdd(id, k => new Dictionary<string, Delegate>());
        if (list.TryGetValue(key, out Delegate old))
            list[key] = Delegate.Combine(old, action);
        else
            list.Add(key, action);
    }

    /// <summary>
    /// 是否C#方法。
    /// </summary>
    /// <param name="id">方法ID。</param>
    public static void Dispose(string id)
    {
        if (actions.Remove(id, out Dictionary<string, Delegate> handlers))
        {
            handlers.Clear();
        }
    }

    /// <summary>
    /// JS异步回调C#方法，不带参数。
    /// </summary>
    /// <param name="id">方法ID。</param>
    /// <param name="key">方法Key。</param>
    /// <returns></returns>
    [JSInvokable]
    public static object CallbackAsync(string id, string key)
    {
        if (actions.TryGetValue(id, out Dictionary<string, Delegate> handlers))
        {
            if (handlers.TryGetValue(key, out Delegate d))
            {
                return d.DynamicInvoke();
            }
        }

        return null;
    }

    /// <summary>
    /// JS异步回调C#方法，带参数。
    /// </summary>
    /// <param name="id">方法ID。</param>
    /// <param name="key">方法Key。</param>
    /// <param name="args">参数字典。</param>
    /// <returns></returns>
    [JSInvokable]
    public static object CallbackByParamAsync(string id, string key, Dictionary<string, object> args)
    {
        if (actions.TryGetValue(id, out Dictionary<string, Delegate> handlers))
        {
            if (handlers.TryGetValue(key, out Delegate d))
            {
                return d.DynamicInvoke(args);
            }
        }

        return null;
    }
}