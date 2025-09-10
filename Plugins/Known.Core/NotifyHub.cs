using Microsoft.AspNetCore.SignalR;

namespace Known;

/// <summary>
/// 后台通知Hub接口。
/// </summary>
public interface INotifyHub
{
    /// <summary>
    /// 发送通知到客户端。
    /// </summary>
    /// <param name="method">方法名。</param>
    /// <param name="message">通知消息。</param>
    /// <returns></returns>
    Task Notify(string method, string message);
}

/// <summary>
/// 后台通知Hub。
/// </summary>
public class NotifyHub : Hub, INotifyHub
{
    /// <summary>
    /// 发送通知到客户端。
    /// </summary>
    /// <param name="method">方法名。</param>
    /// <param name="message">通知消息。</param>
    /// <returns></returns>
    public Task Notify(string method, string message)
    {
        return Clients.All.SendAsync(method, message);
    }
}

class WebNotifyService(IHubContext<NotifyHub> hub) : INotifyService
{
    public string Name => "Web";

    public Task SendAsync<T>(string method, T message, CancellationToken token = default)
    {
        return hub.Clients.All.SendAsync(method, message, token);
    }
}