﻿namespace Known.Services;

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
        return Database.QueryPageAsync<SysRole>(criteria);
    }

    public async Task<Result> DeleteRolesAsync(List<SysRole> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
                await db.DeleteAsync<SysUserRole>(d => d.RoleId == item.Id);
                await db.DeleteAsync<SysRoleModule>(d => d.RoleId == item.Id);
            }
        });
    }

    public async Task<SysRole> GetRoleAsync(string roleId)
    {
        var db = Database;
        await db.OpenAsync();
        var info = string.IsNullOrWhiteSpace(roleId)
                 ? new SysRole()
                 : await db.QueryByIdAsync<SysRole>(roleId);
        info ??= new SysRole();
        info.Modules = await ModuleService.GetModulesAsync(db);
        var roleModules = await db.QueryListAsync<SysRoleModule>(d => d.RoleId == roleId);
        info.MenuIds = roleModules?.Select(d => d.ModuleId).ToList();
        await db.CloseAsync();
        return info;
    }

    public async Task<Result> SaveRoleAsync(SysRole model)
    {
        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            await db.DeleteAsync<SysRoleModule>(d => d.RoleId == model.Id);
            if (model.MenuIds != null && model.MenuIds.Count > 0)
            {
                foreach (var item in model.MenuIds)
                {
                    await db.InsertAsync(new SysRoleModule { RoleId = model.Id, ModuleId = item });
                }
            }
        }, model);
    }
}