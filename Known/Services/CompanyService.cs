namespace Known.Services;

public interface ICompanyService : IService
{
    Task<string> GetCompanyAsync();
    Task<List<SysOrganization>> GetOrganizationsAsync();
    Task<Result> DeleteOrganizationsAsync(List<SysOrganization> models);
    Task<Result> SaveCompanyAsync(object model);
    Task<Result> SaveOrganizationAsync(SysOrganization model);
}

class CompanyService(Context context) : ServiceBase(context), ICompanyService
{
    private const string KeyCompany = "CompanyInfo";

    //Company
    public async Task<string> GetCompanyAsync()
    {
        if (Config.App.IsPlatform)
        {
            return await GetCompanyDataAsync(Database);
        }
        else
        {
            var json = await SystemService.GetConfigAsync(Database, KeyCompany);
            if (string.IsNullOrEmpty(json))
                json = GetDefaultData(Database.User);
            return json;
        }
    }

    public async Task<Result> SaveCompanyAsync(object model)
    {
        if (Config.App.IsPlatform)
        {
            var company = await Database.QueryAsync<SysCompany>(d => d.Code == CurrentUser.CompNo);
            if (company == null)
                return Result.Error(Language["Tip.CompanyNotExists"]);

            company.CompanyData = Utils.ToJson(model);
            await Database.SaveAsync(company);
        }
        else
        {
            await SystemService.SaveConfigAsync(Database, KeyCompany, model);
        }
        return Result.Success(Language.Success(Language.Save));
    }

    private static async Task<string> GetCompanyDataAsync(Database db)
    {
        var company = await db.QueryAsync<SysCompany>(d => d.Code == db.User.CompNo);
        if (company == null)
            return GetDefaultData(db.User);

        var model = company.CompanyData;
        if (string.IsNullOrEmpty(model))
        {
            model = Utils.ToJson(new
            {
                company.Code,
                company.Name,
                company.NameEn,
                company.SccNo,
                company.Address,
                company.AddressEn
            });
        }
        return model;
    }

    private static string GetDefaultData(UserInfo user) => Utils.ToJson(new { Code = user.CompNo, Name = user.CompName });

    //Organization
    public Task<List<SysOrganization>> GetOrganizationsAsync()
    {
        return Database.QueryListAsync<SysOrganization>(d => d.CompNo == CurrentUser.CompNo);
    }

    public async Task<Result> DeleteOrganizationsAsync(List<SysOrganization> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        foreach (var model in models)
        {
            if (await Database.ExistsAsync<SysOrganization>(d => d.ParentId == model.Id))
                return Result.Error(Language["Tip.OrgDeleteExistsChild"]);
        }

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    public async Task<Result> SaveOrganizationAsync(SysOrganization model)
    {
        var vr = model.Validate(Context);
        if (vr.IsValid)
        {
            if (await Database.ExistsAsync<SysOrganization>(d => d.Id != model.Id && d.CompNo == model.CompNo && d.Code == model.Code))
                vr.AddError(Language["Tip.OrgCodeExists"]);
        }
        if (!vr.IsValid)
            return vr;

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            //PlatformHelper.SetBizOrganization(db, entity);
        }, model);
    }
}