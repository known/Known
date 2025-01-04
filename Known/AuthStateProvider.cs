﻿namespace Known;

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
    /// 异步签入当前登录用户信息。
    /// </summary>
    /// <param name="user">当前用户信息。</param>
    /// <returns></returns>
    Task SignInAsync(UserInfo user);

    /// <summary>
    /// 异步签出当前登录用户。
    /// </summary>
    /// <returns></returns>
    Task SignOutAsync();
}

class AuthStateProvider : IAuthStateProvider
{
    private static UserInfo current;

    public Task<UserInfo> GetUserAsync() => Task.FromResult(current);
    public Task SignInAsync(UserInfo user) => SetCurrentUser(user);
    public Task SignOutAsync() => SetCurrentUser(null);

    private static Task SetCurrentUser(UserInfo user)
    {
        current = user;
        return Task.CompletedTask;
    }
}

class JSAuthStateProvider(JSService js) : IAuthStateProvider
{
    public Task<UserInfo> GetUserAsync() => js.GetUserInfoAsync();
    public Task SignInAsync(UserInfo user) => SetCurrentUser(user);
    public Task SignOutAsync() => SetCurrentUser(null);

    private async Task SetCurrentUser(UserInfo user)
    {
        await js.SetUserInfoAsync(user);
    }
}