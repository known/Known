using Microsoft.AspNetCore.SignalR;

namespace Known;

/// <summary>
/// 后台通知Hub。
/// </summary>
public class NotifyHub : Hub
{
    /// <summary>
    /// 注册会话连接。
    /// </summary>
    /// <param name="sessionId">会话ID。</param>
    /// <returns></returns>
    public async Task RegisterSession(string sessionId)
    {
        // 将连接ID与会话ID关联
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
    }

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

class WebNotifyService(IHubContext<NotifyHub> hub, SessionManager session) : INotifyService
{
    public string Name => "Web";

    public Task SendAsync<T>(string method, T info, CancellationToken token = default)
    {
        var json = Utils.ToJson(info);
        return hub.Clients.All.SendAsync(method, json, token);
    }

    public Task SendAsync<T>(string userName, string method, T info, CancellationToken token = default)
    {
        var sessionId = session.GetUserSessionId(userName);
        if (string.IsNullOrWhiteSpace(sessionId))
            return Task.CompletedTask;

        var json = Utils.ToJson(info);
        return hub.Clients.Group(sessionId).SendAsync(method, json, token);
    }
}