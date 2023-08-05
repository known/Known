namespace Known.Razor;

public sealed class CallbackHelper
{
    private static readonly ConcurrentDictionary<string, Dictionary<string, Delegate>> actions = new();

    private CallbackHelper() { }

    public static void Register(string id, string key, Delegate action)
    {
        var list = actions.GetOrAdd(id, k => new Dictionary<string, Delegate>());
        if (list.TryGetValue(key, out Delegate old))
            list[key] = Delegate.Combine(old, action);
        else
            list.Add(key, action);
    }

    public static void Dispose(string id)
    {
        if (actions.Remove(id, out Dictionary<string, Delegate> handlers))
        {
            handlers.Clear();
        }
    }

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