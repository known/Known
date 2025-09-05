namespace Known;

/// <summary>
/// 内存事件总线。
/// </summary>
public static class EventBus
{
    private static readonly Dictionary<string, List<Action<string>>> handlers = [];

    /// <summary>
    /// 消息订阅。
    /// </summary>
    /// <param name="method">方法名。</param>
    /// <param name="handler">处理者。</param>
    public static void Subscribe(string method, Action<string> handler)
    {
        if (!handlers.ContainsKey(method))
            handlers[method] = [];
        handlers[method].Add(handler);
    }

    /// <summary>
    /// 取消消息订阅。
    /// </summary>
    /// <param name="method">方法名。</param>
    /// <param name="handler">处理者。</param>
    public static void Unsubscribe(string method, Action<string> handler)
    {
        if (handlers.TryGetValue(method, out var list))
            list.Remove(handler);
    }

    /// <summary>
    /// 发布消息。
    /// </summary>
    /// <typeparam name="T">消息对象类型。</typeparam>
    /// <param name="method">方法名。</param>
    /// <param name="info">消息对象。</param>
    /// <returns></returns>
    public static void Publish<T>(string method, T info)
    {
        var message = Utils.ToJson(info);
        Publish(method, message);
    }

    /// <summary>
    /// 发布消息。
    /// </summary>
    /// <param name="method">方法名。</param>
    /// <param name="message">消息内容。</param>
    public static void Publish(string method, string message)
    {
        if (handlers.TryGetValue(method, out var list))
        {
            foreach (var handler in list)
                handler(message);
        }
    }
}