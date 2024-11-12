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
}

class UserService(Context context) : ServiceBase(context), IUserService
{
    public Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria)
    {
        throw new NotImplementedException();
    }

    public Task<SysUser> GetUserAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateAvatarAsync(AvatarInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateUserAsync(SysUser model)
    {
        throw new NotImplementedException();
    }
}