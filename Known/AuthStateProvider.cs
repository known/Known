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