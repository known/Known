using Microsoft.AspNetCore.SignalR;

namespace Known;

/// <summary>
/// 后台通知Hub。
/// </summary>
public class NotifyHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> ConnSessions = new();

    /// <summary>
    /// 注册会话连接。
    /// </summary>
    /// <param name="sessionId">会话ID。</param>
    /// <returns></returns>
    public async Task RegisterSession(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            //Console.WriteLine($"[NotifyHub] RegisterSession ignored: empty sessionId, conn={Context.ConnectionId}");
            return;
        }

        if (ConnSessions.TryGetValue(Context.ConnectionId, out var oldSessionId) &&
            !string.Equals(oldSessionId, sessionId, StringComparison.OrdinalIgnoreCase))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, oldSessionId);
            //Console.WriteLine($"[NotifyHub] RegisterSession remove old: conn={Context.ConnectionId}, oldSessionId={oldSessionId}");
        }

        ConnSessions[Context.ConnectionId] = sessionId;
        //Console.WriteLine($"[NotifyHub] RegisterSession: conn={Context.ConnectionId}, sessionId={sessionId}");
        // 将连接ID与会话ID关联
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
    }

    /// <inheritdoc />
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (ConnSessions.TryRemove(Context.ConnectionId, out var sessionId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
            //Console.WriteLine($"[NotifyHub] OnDisconnected remove: conn={Context.ConnectionId}, sessionId={sessionId}");
        }

        await base.OnDisconnectedAsync(exception);
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
        {
            //Console.WriteLine($"[NotifyHub] SendAsync skipped: user={userName}, method={method}, reason=no-session");
            return Task.CompletedTask;
        }

        //Console.WriteLine($"[NotifyHub] SendAsync: user={userName}, method={method}, sessionId={sessionId}");
        var json = Utils.ToJson(info);
        return hub.Clients.Group(sessionId).SendAsync(method, json, token);
    }
}