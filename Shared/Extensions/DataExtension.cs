namespace Known.Extensions;

/// <summary>
/// 数据访问扩展类。
/// </summary>
public static class DataExtension
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

    /// <summary>
    /// 异步添加操作日志。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="type">日志类型。</param>
    /// <param name="target">操作对象。</param>
    /// <param name="content">操作内容。</param>
    /// <returns></returns>
    public static Task AddLogAsync(this Database db, LogType type, string target, string content)
    {
        return db.AddLogAsync(new LogInfo { Type = type.ToString(), Target = target, Content = content });
    }
}