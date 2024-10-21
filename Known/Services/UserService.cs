namespace Known.Services;

/// <summary>
/// 系统用户服务接口。
/// </summary>
public interface IUserService : IService
{
    /// <summary>
    /// 异步分页查询系统用户。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取系统用户。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <returns>系统用户。</returns>
    Task<SysUser> GetUserAsync(string id);

    /// <summary>
    /// 异步获取系统用户数据。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <returns>系统用户数据。</returns>
    Task<SysUser> GetUserDataAsync(string id);

    /// <summary>
    /// 异步删除系统用户。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteUsersAsync(List<SysUser> models);

    /// <summary>
    /// 异步切换系统用户所属部门。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>切换结果。</returns>
    Task<Result> ChangeDepartmentAsync(List<SysUser> models);

    /// <summary>
    /// 异步启用系统用户。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>启用结果。</returns>
    Task<Result> EnableUsersAsync(List<SysUser> models);

    /// <summary>
    /// 异步禁用系统用户。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>禁用结果。</returns>
    Task<Result> DisableUsersAsync(List<SysUser> models);

    /// <summary>
    /// 异步重置系统用户密码。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>重置结果。</returns>
    Task<Result> SetUserPwdsAsync(List<SysUser> models);

    /// <summary>
    /// 异步修改系统用户头像。
    /// </summary>
    /// <param name="info">用户头像信息。</param>
    /// <returns>修改结果。</returns>
    Task<Result> UpdateAvatarAsync(AvatarInfo info);

    /// <summary>
    /// 异步修改系统用户信息。
    /// </summary>
    /// <param name="model">系统用户信息。</param>
    /// <returns>修改结果。</returns>
    Task<Result> UpdateUserAsync(SysUser model);

    /// <summary>
    /// 异步保存系统用户。
    /// </summary>
    /// <param name="model">系统用户信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserAsync(SysUser model);
}