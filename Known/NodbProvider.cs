namespace Known;

/// <summary>
/// 无数据库提供者接口。
/// </summary>
public interface INodbProvider
{
    /// <summary>
    /// 异步获取当前用户菜单信息列表。
    /// </summary>
    /// <param name="user">当前用户。</param>
    /// <returns>菜单信息列表。</returns>
    Task<List<MenuInfo>> GetUserMenusAsync(UserInfo user);

    /// <summary>
    /// 异步获取缓存代码表列表。
    /// </summary>
    /// <param name="user">当前用户。</param>
    /// <returns>代码表列表。</returns>
    Task<List<CodeInfo>> GetCodesAsync(UserInfo user);
}

class NodbProvider : INodbProvider
{
    public Task<List<MenuInfo>> GetUserMenusAsync(UserInfo user) => Task.FromResult(new List<MenuInfo>());
    public Task<List<CodeInfo>> GetCodesAsync(UserInfo user) => Task.FromResult(new List<CodeInfo>());
}