namespace Known.Admin.Services;

/// <summary>
/// 组织架构服务接口。
/// </summary>
public interface IOrganizationService : IService
{
    /// <summary>
    /// 异步获取组织架构列表。
    /// </summary>
    /// <returns>组织架构列表。</returns>
    Task<List<SysOrganization>> GetOrganizationsAsync();

    /// <summary>
    /// 异步删除组织架构信息。
    /// </summary>
    /// <param name="models">组织架构列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteOrganizationsAsync(List<SysOrganization> models);

    /// <summary>
    /// 异步保存组织架构信息。
    /// </summary>
    /// <param name="model">组织架构信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveOrganizationAsync(SysOrganization model);
}

class OrganizationService(Context context) : ServiceBase(context), IOrganizationService
{
    public Task<List<SysOrganization>> GetOrganizationsAsync()
    {
        return Database.QueryListAsync<SysOrganization>(d => d.CompNo == CurrentUser.CompNo);
    }

    public async Task<Result> DeleteOrganizationsAsync(List<SysOrganization> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        foreach (var model in models)
        {
            if (await database.ExistsAsync<SysOrganization>(d => d.ParentId == model.Id))
                return Result.Error(Language["Tip.OrgDeleteExistsChild"]);
        }

        return await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    public async Task<Result> SaveOrganizationAsync(SysOrganization model)
    {
        var database = Database;
        var vr = model.Validate(Context);
        if (vr.IsValid)
        {
            if (await database.ExistsAsync<SysOrganization>(d => d.Id != model.Id && d.CompNo == model.CompNo && d.Code == model.Code))
                vr.AddError(Language["Tip.OrgCodeExists"]);
        }
        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            //PlatformHelper.SetBizOrganization(db, entity);
        }, model);
    }
}