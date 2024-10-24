namespace Known.Services;

/// <summary>
/// 系统角色服务接口。
/// </summary>
public interface IRoleService : IService
{
    /// <summary>
    /// 异步分页查询系统角色。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取系统角色。
    /// </summary>
    /// <param name="roleId">角色ID。</param>
    /// <returns>系统角色。</returns>
    Task<SysRole> GetRoleAsync(string roleId);

    /// <summary>
    /// 异步删除系统角色。
    /// </summary>
    /// <param name="models">系统角色列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteRolesAsync(List<SysRole> models);

    /// <summary>
    /// 异步保存系统角色。
    /// </summary>
    /// <param name="model">系统角色信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveRoleAsync(SysRole model);
}

class RoleService(Context context) : ServiceBase(context), IRoleService
{
    public Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria)
    {
        throw new NotImplementedException();
    }

    public Task<SysRole> GetRoleAsync(string roleId)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteRolesAsync(List<SysRole> models)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveRoleAsync(SysRole model)
    {
        throw new NotImplementedException();
    }
}