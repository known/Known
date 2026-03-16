using Microsoft.AspNetCore.SignalR;

namespace Known.Helpers;

class SessionManager(IHubContext<NotifyHub> hubContext)
{
    // 存储用户ID和会话信息的映射
    private readonly ConcurrentDictionary<string, UserInfo> _sessions = new();
    private readonly IHubContext<NotifyHub> _hubContext = hubContext;

    // 添加新会话（兼容同步调用）
    public string CreateSession(UserInfo info)
    {
        return CreateSessionAsync(info).GetAwaiter().GetResult();
    }

    // 添加新会话
    public async Task<string> CreateSessionAsync(UserInfo info)
    {
        if (info == null)
            return string.Empty;

        CoreConfig.Systems.TryGetValue(info.CompNo, out SystemInfo sys);
        info.SessionId = Utils.GetGuid();
        //Console.WriteLine($"[Session] CreateSession: user={info.UserName}, compNo={info.CompNo}, newSession={info.SessionId}, isLoginOne={sys?.IsLoginOne}");

        // 仅在单账号登录开启时踢掉旧会话
        if (sys?.IsLoginOne == true && _sessions.TryRemove(info.UserName, out var session))
        {
            //Console.WriteLine($"[Session] ForceLogout send start: user={info.UserName}, oldSession={session.SessionId}, newSession={info.SessionId}");
            await _hubContext.Clients.Group(session.SessionId).SendAsync(Constants.ForceLogout, sys?.TipLoginOne);
            //Console.WriteLine($"[Session] ForceLogout send done: user={info.UserName}, oldSession={session.SessionId}");
        }

        _sessions[info.UserName] = info;
        return info.SessionId;
    }

    // 验证会话是否有效
    public bool ValidateSession(string username, string sessionId)
    {
        if (_sessions.TryGetValue(username, out var session))
        {
            var isValid = session.SessionId == sessionId;
            //if (!isValid)
            //    Console.WriteLine($"[Session] Validate failed: user={username}, input={sessionId}, current={session.SessionId}");
            return isValid;
        }

        //Console.WriteLine($"[Session] Validate failed: user={username}, reason=not-found, input={sessionId}");
        return false;
    }

    // 移除会话
    public void RemoveSession(string username)
    {
        _sessions.TryRemove(username, out _);
    }

    // 获取用户会话ID
    public string GetUserSessionId(string userName)
    {
        return _sessions.TryGetValue(userName, out var session) ? session.SessionId : null;
    }
}