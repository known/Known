namespace Known;

/// <summary>
/// 用户处理者接口。
/// </summary>
public interface IUserHandler
{
    /// <summary>
    /// 异步删除前处理。
    /// </summary>
    /// <param name="db">数据库实例。</param>
    /// <param name="infos">用户对象列表。</param>
    /// <returns></returns>
    Task<Result> OnDeletingAsync(Database db, List<UserInfo> infos);

    /// <summary>
    /// 异步删除后处理。
    /// </summary>
    /// <param name="db">数据库实例。</param>
    /// <param name="info">用户对象。</param>
    /// <returns></returns>
    Task OnDeletedAsync(Database db, UserInfo info);

    /// <summary>
    /// 异步改变部门前处理。
    /// </summary>
    /// <param name="db">数据库实例。</param>
    /// <param name="infos">用户对象列表。</param>
    /// <returns></returns>
    Task<Result> OnChangingDepartmentAsync(Database db, List<UserInfo> infos);

    /// <summary>
    /// 异步改变部门后处理。
    /// </summary>
    /// <param name="db">数据库实例。</param>
    /// <param name="info">用户对象。</param>
    /// <returns></returns>
    Task OnChangedDepartmentAsync(Database db, SysUser info);

    /// <summary>
    /// 异步启用前处理。
    /// </summary>
    /// <param name="db">数据库实例。</param>
    /// <param name="infos">用户对象列表。</param>
    /// <returns></returns>
    Task<Result> OnEnablingAsync(Database db, List<UserInfo> infos);

    /// <summary>
    /// 异步启用后处理。
    /// </summary>
    /// <param name="db">数据库实例。</param>
    /// <param name="info">用户对象。</param>
    /// <returns></returns>
    Task OnEnabledAsync(Database db, SysUser info);

    /// <summary>
    /// 异步禁用前处理。
    /// </summary>
    /// <param name="db">数据库实例。</param>
    /// <param name="infos">用户对象。</param>
    /// <returns></returns>
    Task<Result> OnDisablingAsync(Database db, List<UserInfo> infos);

    /// <summary>
    /// 异步禁用后处理。
    /// </summary>
    /// <param name="db">数据库实例。</param>
    /// <param name="info">用户对象。</param>
    /// <returns></returns>
    Task OnDisabledAsync(Database db, SysUser info);

    /// <summary>
    /// 异步保存前处理。
    /// </summary>
    /// <param name="db">数据库实例。</param>
    /// <param name="info">用户对象。</param>
    /// <returns></returns>
    Task<Result> OnSavingAsync(Database db, UserInfo info);

    /// <summary>
    /// 异步保存后处理者。
    /// </summary>
    /// <param name="db">数据库实例。</param>
    /// <param name="info">用户对象。</param>
    /// <returns></returns>
    Task OnSavedAsync(Database db, SysUser info);
}

class UserHandler : IUserHandler
{
    public Task<Result> OnDeletingAsync(Database db, List<UserInfo> infos) => Result.SuccessAsync("");
    public Task OnDeletedAsync(Database db, UserInfo info) => Task.CompletedTask;
    public Task<Result> OnChangingDepartmentAsync(Database db, List<UserInfo> infos) => Result.SuccessAsync("");
    public Task OnChangedDepartmentAsync(Database db, SysUser info) => Task.CompletedTask;
    public Task<Result> OnEnablingAsync(Database db, List<UserInfo> infos) => Result.SuccessAsync("");
    public Task OnEnabledAsync(Database db, SysUser info) => Task.CompletedTask;
    public Task<Result> OnDisablingAsync(Database db, List<UserInfo> infos) => Result.SuccessAsync("");
    public Task OnDisabledAsync(Database db, SysUser info) => Task.CompletedTask;
    public Task<Result> OnSavingAsync(Database db, UserInfo info) => Result.SuccessAsync("");
    public Task OnSavedAsync(Database db, SysUser info) => Task.CompletedTask;
}