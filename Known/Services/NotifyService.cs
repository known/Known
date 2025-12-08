namespace Known.Services;

/// <summary>
/// 消息通知服务接口。
/// </summary>
public interface INotifyService
{
    /// <summary>
    /// 取得服务实现类名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 发送通知消息。
    /// </summary>
    /// <typeparam name="T">消息类型。</typeparam>
    /// <param name="method">方法名。</param>
    /// <param name="info">消息对象。</param>
    /// <param name="token">取消Token。</param>
    /// <returns></returns>
    Task SendAsync<T>(string method, T info, CancellationToken token = default);

    /// <summary>
    /// 发送通知消息。
    /// </summary>
    /// <typeparam name="T">消息类型。</typeparam>
    /// <param name="userName">用户名。</param>
    /// <param name="method">方法名。</param>
    /// <param name="info">消息对象。</param>
    /// <param name="token">取消Token。</param>
    /// <returns></returns>
    Task SendAsync<T>(string userName, string method, T info, CancellationToken token = default);
}

class NotifyService : INotifyService
{
    public string Name => "Def";

    public Task SendAsync<T>(string method, T info, CancellationToken token = default)
    {
        EventBus.Publish(method, info);
        return Task.CompletedTask;
    }

    public Task SendAsync<T>(string userName, string method, T info, CancellationToken token = default)
    {
        EventBus.Publish(method, info);
        return Task.CompletedTask;
    }
}