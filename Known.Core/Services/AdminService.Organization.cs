namespace Known.Services;

partial class AdminService
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