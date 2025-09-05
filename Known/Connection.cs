namespace Known;

/// <summary>
/// 数据实时通讯连接接口。
/// </summary>
public interface IConnection : IAsyncDisposable
{
    /// <summary>
    /// 取得连接实现类名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 异步启动指定消息连接。
    /// </summary>
    /// <typeparam name="T">消息类型。</typeparam>
    /// <param name="hubUrl">Hub地址。</param>
    /// <param name="method">消息方法名。</param>
    /// <param name="handler">消息处理者。</param>
    /// <param name="token">任务取消Token。</param>
    /// <returns></returns>
    Task StartAsync<T>(string hubUrl, string method, Action<T> handler, CancellationToken token = default);

    /// <summary>
    /// 异步停止指定消息连接。
    /// </summary>
    /// <param name="method">消息方法名。</param>
    /// <param name="token">任务取消Token。</param>
    /// <returns></returns>
    Task StopAsync(string method, CancellationToken token = default);

    /// <summary>
    /// 异步停止所有消息连接。
    /// </summary>
    /// <param name="token">任务取消Token。</param>
    /// <returns></returns>
    Task StopAsync(CancellationToken token = default);
}

class Connection : IConnection
{
    private readonly Dictionary<string, Action<string>> localHandlers = [];

    public string Name => "MEB";

    public Task StartAsync<T>(string hubUrl, string method, Action<T> handler, CancellationToken token = default)
    {
        localHandlers[method] = message =>
        {
            var info = Utils.FromJson<T>(message);
            handler.Invoke(info);
        };
        EventBus.Subscribe(method, localHandlers[method]);
        return Task.CompletedTask;
    }

    public Task StopAsync(string method, CancellationToken token = default)
    {
        if (localHandlers.TryGetValue(method, out var handler))
        {
            EventBus.Unsubscribe(method, handler);
            localHandlers.Remove(method);
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken token = default)
    {
        foreach (var kv in localHandlers)
        {
            EventBus.Unsubscribe(kv.Key, kv.Value);
        }
        localHandlers.Clear();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask(StopAsync());
    }
}