namespace Known;

/// <summary>
/// 无数据库提供者接口。
/// </summary>
public interface INodbProvider
{
    /// <summary>
    /// 获取当前用户菜单信息列表。
    /// </summary>
    /// <param name="user">当前用户。</param>
    /// <returns>菜单信息列表。</returns>
    List<MenuInfo> GetUserMenus(UserInfo user);

    /// <summary>
    /// 获取缓存代码表列表。
    /// </summary>
    /// <returns>代码表列表。</returns>
    List<CodeInfo> GetCodes();
}

class NodbProvider : INodbProvider
{
    public List<MenuInfo> GetUserMenus(UserInfo user) => [];
    public List<CodeInfo> GetCodes() => [];
}