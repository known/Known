﻿namespace Known.Services;

class CompanyService : BaseService
{
    private const string KeyCompany = "CompanyInfo";

    //Company
    internal static async Task<string> GetCompanyAsync(Database db, UserInfo user)
    {
        if (Config.IsPlatform)
            return await GetCompanyDataAsync(db, user);

        var model = await PlatformRepository.GetConfigAsync(db, Config.AppId, KeyCompany);
        if (string.IsNullOrEmpty(model))
            model = GetDefaultData(user);
        return model;
    }

    public async Task<T> GetCompanyAsync<T>()
    {
        var user = CurrentUser;
        var json = await GetCompanyAsync(Database, user);
        return Utils.FromJson<T>(json);
    }

    public async Task<Result> SaveCompanyAsync(object model)
    {
        var user = CurrentUser;
        if (Config.IsPlatform)
        {
            var company = await CompanyRepository.GetCompanyAsync(Database, user.CompNo);
            if (company == null)
                return Result.Error("企业不存在！");

            company.CompanyData = Utils.ToJson(model);
            await Database.SaveAsync(company);
        }
        else
        {
            await SaveConfigAsync(Database, KeyCompany, model);
        }
        return Result.Success("保存成功！");
    }

    private static async Task<string> GetCompanyDataAsync(Database db, UserInfo user)
    {
        var company = await CompanyRepository.GetCompanyAsync(db, user.CompNo);
        if (company == null)
            return GetDefaultData(user);

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
                return Result.Error("存在子组织架构，不能删除！");
        }

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    public async Task<Result> SaveOrganizationAsync(dynamic model)
    {
        var entity = await Database.QueryByIdAsync<SysOrganization>((string)model.Id);
        entity ??= new SysOrganization();
        entity.FillModel(model);
        var vr = entity.Validate();
        if (vr.IsValid)
        {
            if (await CompanyRepository.ExistsOrganizationAsync(Database, entity))
                vr.AddError("组织编码已存在！");
        }
        if (!vr.IsValid)
            return vr;

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(entity);
            PlatformHelper.SetBizOrganization(db, entity);
        }, entity);
    }
}