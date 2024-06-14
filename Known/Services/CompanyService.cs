namespace Known.Services;

public interface ICompanyService : IService
{
    Task<T> GetCompanyAsync<T>();
    Task<List<SysOrganization>> GetOrganizationsAsync();
    Task<Result> DeleteOrganizationsAsync(List<SysOrganization> models);
    Task<Result> SaveCompanyAsync(object model);
    Task<Result> SaveOrganizationAsync(SysOrganization model);
}

class CompanyService(Context context) : ServiceBase(context), ICompanyService
{
    private const string KeyCompany = "CompanyInfo";

    //Company
    public async Task<T> GetCompanyAsync<T>()
    {
        var json = string.Empty;
        if (Config.App.IsPlatform)
        {
            json = await GetCompanyDataAsync(Database);
        }
        else
        {
            var model = await SystemRepository.GetConfigAsync(Database, KeyCompany);
            if (string.IsNullOrEmpty(model))
                json = GetDefaultData(Database.User);
        }
        return Utils.FromJson<T>(json);
    }

    public async Task<Result> SaveCompanyAsync(object model)
    {
        if (Config.App.IsPlatform)
        {
            var company = await CompanyRepository.GetCompanyAsync(Database);
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
        var company = await CompanyRepository.GetCompanyAsync(db);
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
    public Task<List<SysOrganization>> GetOrganizationsAsync() => CompanyRepository.GetOrganizationsAsync(Database);

    public async Task<Result> DeleteOrganizationsAsync(List<SysOrganization> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        foreach (var model in models)
        {
            if (await CompanyRepository.ExistsSubOrganizationAsync(Database, model.Id))
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
            if (await CompanyRepository.ExistsOrganizationAsync(Database, model))
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