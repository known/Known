namespace Known.Extensions;

/// <summary>
/// 数据权限扩展类。
/// </summary>
public static class PurviewExtension
{
    /// <summary>
    /// 异步设置数据权限。
    /// </summary>
    /// <typeparam name="T">数据权限类型。</typeparam>
    /// <param name="db">数据库对象。</param>
    /// <param name="userId">用户ID。</param>
    /// <returns></returns>
    public static async Task<T> GetDataPurviewAsync<T>(this Database db, string userId)
    {
        var user = await db.QueryByIdAsync<SysUser>(userId);
        if (user == null || string.IsNullOrWhiteSpace(user.Data))
            return default;

        var info = Utils.FromJson<T>(user.Data);
        if (info == null)
            return default;

        return info;
    }
}