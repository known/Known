using Microsoft.AspNetCore.SignalR;

namespace Known.Helpers;

class SessionManager
{
    // 存储用户ID和会话信息的映射
    private readonly ConcurrentDictionary<string, UserInfo> _sessions = new();
    private readonly IHubContext<NotifyHub> _hubContext;

    public SessionManager(IHubContext<NotifyHub> hubContext)
    {
        _hubContext = hubContext;
    }

    // 添加新会话
    public string CreateSession(UserInfo info)
    {
        var sys = CoreConfig.System;
        if (info == null || sys?.IsLoginOne == false)
            return string.Empty;

        info.SessionId = Utils.GetGuid();
        // 如果用户已有活跃会话，移除旧会话
        if (_sessions.TryRemove(info.UserName, out var session))
            _hubContext.Clients.Group(session.SessionId).SendAsync("ForceLogout", sys?.TipLoginOne);

        _sessions[info.UserName] = info;
        return info.SessionId;
    }

    // 验证会话是否有效
    public bool ValidateSession(string username, string sessionId)
    {
        if (_sessions.TryGetValue(username, out var session))
        {
            return session.SessionId == sessionId;
        }
        return false;
    }

    // 移除会话
    public void RemoveSession(string username)
    {
        _sessions.TryRemove(username, out _);
    }

    // 获取用户会话ID
    public string GetUserSessionId(string username)
    {
        return _sessions.TryGetValue(username, out var session) ? session.SessionId : null;
    }
}