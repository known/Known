namespace Known.Extensions;

/// <summary>
/// 用户数据扩展类。
/// </summary>
public static class UserExtension
{
    /// <summary>
    /// 取得或设置根据用户名获取用户信息的异步委托。
    /// </summary>
    public static Func<Database, string, Task<UserInfo>> OnGetUser { get; set; } = (db, userName) => Task.FromResult(default(UserInfo));

    /// <summary>
    /// 取得或设置根据用户名获取用户信息的异步委托。
    /// </summary>
    public static Func<Database, string, string, Task<UserInfo>> OnGetUserByPwd { get; set; } = (db, userName, password) => Task.FromResult(default(UserInfo));

    /// <summary>
    /// 取得或设置根据用户ID获取用户信息的异步委托。
    /// </summary>
    public static Func<Database, string, Task<UserInfo>> OnGetUserById { get; set; } = (db, id) => Task.FromResult(default(UserInfo));

    /// <summary>
    /// 取得或设置根据角色获取用户信息列表的异步委托。
    /// </summary>
    public static Func<Database, string, Task<List<UserInfo>>> OnGetUsersByRole { get; set; } = (db, roleName) => Task.FromResult(new List<UserInfo>());

    /// <summary>
    /// 取得或设置添加用户的异步委托。
    /// </summary>
    public static Func<Database, UserDataInfo, Task<Result>> OnAddUser { get; set; } = (db, info) => Result.SuccessAsync("");

    /// <summary>
    /// 取得或设置保存用户的异步委托。
    /// </summary>
    public static Func<Database, Context, UserInfo, Task<Result>> OnSaveUser { get; set; } = (db, ctx, info) => Result.SuccessAsync("");

    /// <summary>
    /// 取得或设置同步用户的异步委托。
    /// </summary>
    public static Func<Database, UserInfo, Task<Result>> OnSyncUser { get; set; } = (db, info) => Result.SuccessAsync("");

    /// <summary>
    /// 取得或设置修改用户头像的异步委托。
    /// </summary>
    public static Func<Database, AvatarInfo, Task<Result>> OnUpdateAvatar { get; set; } = (db, info) => Result.SuccessAsync("");

    /// <summary>
    /// 取得或设置修改用户密码的异步委托。
    /// </summary>
    public static Func<Database, PwdFormInfo, Task<Result>> OnUpdatePassword { get; set; } = (db, info) => Result.SuccessAsync("");

    /// <summary>
    /// 异步根据用户名获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <returns>用户信息。</returns>
    public static Task<UserInfo> GetUserAsync(this Database db, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Task.FromResult(default(UserInfo));

        userName = userName.ToLower();
        return OnGetUser.Invoke(db, userName);
    }

    /// <summary>
    /// 根据用户名和密码获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="password">密码。</param>
    /// <returns></returns>
    public static Task<UserInfo> GetUserAsync(this Database db, string userName, string password)
    {
        userName = userName.ToLower();
        return OnGetUserByPwd.Invoke(db, userName, password);
    }

    /// <summary>
    /// 异步根据用户ID获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="id">用户ID。</param>
    /// <returns>用户信息。</returns>
    public static Task<UserInfo> GetUserByIdAsync(this Database db, string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Task.FromResult(default(UserInfo));

        return OnGetUserById.Invoke(db, id);
    }

    /// <summary>
    /// 异步获取角色用户列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="roleName">角色名称。</param>
    /// <returns>用户列表。</returns>
    public static Task<List<UserInfo>> GetUsersByRoleAsync(this Database db, string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return Task.FromResult(new List<UserInfo>());

        return OnGetUsersByRole.Invoke(db, roleName);
    }

    /// <summary>
    /// 异步添加系统用户。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">系统用户对象。</param>
    /// <returns></returns>
    public static Task<Result> AddUserAsync(this Database db, UserDataInfo info) => OnAddUser.Invoke(db, info);

    /// <summary>
    /// 异步保存系统用户。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="context">系统上下文。</param>
    /// <param name="info">系统用户对象。</param>
    /// <returns></returns>
    public static Task<Result> SaveUserAsync(this Database db, Context context, UserInfo info) => OnSaveUser.Invoke(db, context, info);

    /// <summary>
    /// 异步同步系统用户到框架用户表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">系统用户对象。</param>
    /// <returns></returns>
    public static Task<Result> SyncUserAsync(this Database db, UserInfo info) => OnSyncUser.Invoke(db, info);

    /// <summary>
    /// 异步修改用户头像。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">用户头像信息。</param>
    /// <returns></returns>
    public static Task<Result> UpdateAvatarAsync(this Database db, AvatarInfo info) => OnUpdateAvatar.Invoke(db, info);

    /// <summary>
    /// 异步修改用户密码。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">密码表单信息。</param>
    /// <returns></returns>
    public static Task<Result> UpdatePasswordAsync(this Database db, PwdFormInfo info) => OnUpdatePassword.Invoke(db, info);
}