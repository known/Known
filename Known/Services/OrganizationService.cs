namespace Known.Services;

/// <summary>
/// 组织架构服务接口。
/// </summary>
public interface IOrganizationService : IService
{
    /// <summary>
    /// 异步获取组织架构列表。
    /// </summary>
    /// <returns></returns>
    Task<List<SysOrganization>> GetOrganizationsAsync();

    /// <summary>
    /// 异步删除组织架构。
    /// </summary>
    /// <param name="infos">组织架构列表。</param>
    /// <returns></returns>
    Task<Result> DeleteOrganizationsAsync(List<SysOrganization> infos);

    /// <summary>
    /// 异步保存组织架构。
    /// </summary>
    /// <param name="info">组织架构信息。</param>
    /// <returns></returns>
    Task<Result> SaveOrganizationAsync(SysOrganization info);
}

[Client]
class OrganizationClient(HttpClient http) : ClientBase(http), IOrganizationService
{
    public Task<List<SysOrganization>> GetOrganizationsAsync() => Http.GetAsync<List<SysOrganization>>("/Organization/GetOrganizations");
    public Task<Result> DeleteOrganizationsAsync(List<SysOrganization> infos) => Http.PostAsync("/Organization/DeleteOrganizations", infos);
    public Task<Result> SaveOrganizationAsync(SysOrganization info) => Http.PostAsync("/Organization/SaveOrganization", info);
}

[WebApi, Service]
class OrganizationService(Context context) : ServiceBase(context), IOrganizationService
{
    public Task<List<SysOrganization>> GetOrganizationsAsync()
    {
        return Database.QueryListAsync<SysOrganization>(d => d.CompNo == CurrentUser.CompNo);
    }

    public async Task<Result> DeleteOrganizationsAsync(List<SysOrganization> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        foreach (var item in infos)
        {
            if (await database.ExistsAsync<SysOrganization>(d => d.ParentId == item.Id))
                return Result.Error(Language.TipOrgDeleteExistsChild);
        }

        return await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysOrganization>(item.Id);
            }
        });
    }

    public async Task<Result> SaveOrganizationAsync(SysOrganization info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysOrganization>(info.Id);
        model ??= new SysOrganization();
        model.FillModel(info);

        var vr = model.Validate(Context);
        if (vr.IsValid)
        {
            if (await database.ExistsAsync<SysOrganization>(d => d.Id != model.Id && d.CompNo == model.CompNo && d.Code == model.Code))
                vr.AddError(Language[Language.TipOrgCodeExists]);
        }
        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            //PlatformHelper.SetBizOrganization(db, entity);
            info.Id = model.Id;
        }, info);
    }
}