namespace Known.Extensions;

/// <summary>
/// 数据权限扩展类。
/// </summary>
public static class PurviewExtension
{
    /// <summary>
    /// 取得或设置根据用户ID获取用户信息的异步委托。
    /// </summary>
    public static Func<Database, string, Task<string>> OnGetUserData { get; set; } = (db, id) => Task.FromResult("");

    /// <summary>
    /// 异步设置数据权限。
    /// </summary>
    /// <typeparam name="T">数据权限类型。</typeparam>
    /// <param name="db">数据库对象。</param>
    /// <param name="userId">用户ID。</param>
    /// <returns></returns>
    public static async Task<T> GetDataPurviewAsync<T>(this Database db, string userId)
    {
        var data = await OnGetUserData.Invoke(db, userId);
        if (string.IsNullOrWhiteSpace(data))
            return default;

        var info = Utils.FromJson<T>(data);
        if (info == null)
            return default;

        return info;
    }
}