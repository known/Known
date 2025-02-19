namespace Known.Services;

partial class AdminService
{
    public Task<List<OrganizationInfo>> GetOrganizationsAsync()
    {
        return Database.Query<SysOrganization>()
                       .Where(d => d.CompNo == CurrentUser.CompNo)
                       .ToListAsync<OrganizationInfo>();
    }

    public async Task<Result> DeleteOrganizationsAsync(List<OrganizationInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        foreach (var item in infos)
        {
            if (await database.ExistsAsync<SysOrganization>(d => d.ParentId == item.Id))
                return Result.Error(Language["Tip.OrgDeleteExistsChild"]);
        }

        return await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysOrganization>(item.Id);
            }
        });
    }

    public async Task<Result> SaveOrganizationAsync(OrganizationInfo info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysOrganization>(info.Id);
        model ??= new SysOrganization();
        model.FillModel(info);

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
        }, info);
    }
}