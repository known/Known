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
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task<bool> SetDataPurviewAsync<T>(this Database db, Action<T> action)
    {
        var user = await db.QueryByIdAsync<SysUser>(db.User.Id);
        if (user == null || string.IsNullOrWhiteSpace(user.Data))
            return false;

        var info = Utils.FromJson<T>(user.Data);
        if (info == null)
            return false;

        action.Invoke(info);
        return true;
    }
}