namespace Known;

/// <summary>
/// 身份认证状态提供者接口。
/// </summary>
public interface IAuthStateProvider
{
    /// <summary>
    /// 异步获取当前用户信息。
    /// </summary>
    /// <returns>当前用户信息。</returns>
    Task<UserInfo> GetUserAsync();

    /// <summary>
    /// 异步设置当前登录用户信息。
    /// </summary>
    /// <param name="user">当前用户信息。</param>
    /// <returns></returns>
    Task SetUserAsync(UserInfo user);
}

class AuthStateProvider : IAuthStateProvider
{
    private static UserInfo current;

    public Task<UserInfo> GetUserAsync() => Task.FromResult(current);

    public Task SetUserAsync(UserInfo user)
    {
        current = user;
        return Task.CompletedTask;
    }

    private static ClaimsPrincipal GetPrincipal(UserInfo user)
    {
        if (user == null)
            return new(new ClaimsIdentity());

        return user.ToPrincipal();
    }
}