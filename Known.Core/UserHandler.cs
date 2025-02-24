namespace Known;

/// <summary>
/// 自定义用户业务逻辑处理者类。
/// </summary>
public class UserHandler
{
    /// <summary>
    /// 删除前异步验证。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="infos">用户信息列表。</param>
    /// <returns></returns>
    public virtual Task<Result> OnDeletingAsync(Database db, List<UserInfo> infos) => Result.SuccessAsync("");

    /// <summary>
    /// 删除后异步操作。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">用户信息。</param>
    /// <returns></returns>
    public virtual Task OnDeletedAsync(Database db, UserInfo info) => Task.CompletedTask;

    /// <summary>
    /// 切换部门前异步验证。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="infos">用户信息列表。</param>
    /// <returns></returns>
    public virtual Task<Result> OnChangingDepartmentAsync(Database db, List<UserInfo> infos) => Result.SuccessAsync("");

    /// <summary>
    /// 切换部门后异步操作。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">用户信息。</param>
    /// <returns></returns>
    public virtual Task OnChangedDepartmentAsync(Database db, SysUser info) => Task.CompletedTask;

    /// <summary>
    /// 启用用户前异步验证。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="infos">用户信息列表。</param>
    /// <returns></returns>
    public virtual Task<Result> OnEnablingAsync(Database db, List<UserInfo> infos) => Result.SuccessAsync("");

    /// <summary>
    /// 启用用户后异步操作。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">用户信息。</param>
    /// <returns></returns>
    public virtual Task OnEnabledAsync(Database db, SysUser info) => Task.CompletedTask;

    /// <summary>
    /// 禁用用户前异步验证。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="infos">用户信息列表。</param>
    /// <returns></returns>
    public virtual Task<Result> OnDisablingAsync(Database db, List<UserInfo> infos) => Result.SuccessAsync("");

    /// <summary>
    /// 禁用用户后异步操作。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">用户信息。</param>
    /// <returns></returns>
    public virtual Task OnDisabledAsync(Database db, SysUser info) => Task.CompletedTask;

    /// <summary>
    /// 保存用户前异步验证。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">用户信息。</param>
    /// <returns></returns>
    public virtual Task<Result> OnSavingAsync(Database db, UserInfo info) => Result.SuccessAsync("");

    /// <summary>
    /// 保存用户后异步操作。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">用户信息。</param>
    /// <returns></returns>
    public virtual Task OnSavedAsync(Database db, SysUser info) => Task.CompletedTask;
}