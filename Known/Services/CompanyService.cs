﻿namespace Known.Services;

/// <summary>
/// 系统租户服务接口。
/// </summary>
public interface ICompanyService : IService
{
    /// <summary>
    /// 异步获取租户企业信息。
    /// </summary>
    /// <returns>企业信息JSO你。</returns>
    Task<string> GetCompanyAsync();

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
    /// 异步保存企业信息。
    /// </summary>
    /// <param name="model">企业信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveCompanyAsync(object model);

    /// <summary>
    /// 异步保存组织架构信息。
    /// </summary>
    /// <param name="model">组织架构信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveOrganizationAsync(SysOrganization model);
}

class CompanyService(Context context) : ServiceBase(context), ICompanyService
{
    private const string KeyCompany = "CompanyInfo";

    //Company
    public async Task<string> GetCompanyAsync()
    {
        var database = Database;
        if (Config.App.IsPlatform)
        {
            return await GetCompanyDataAsync(database);
        }
        else
        {
            var json = await SystemService.GetConfigAsync(database, KeyCompany);
            if (string.IsNullOrEmpty(json))
                json = GetDefaultData(database.User);
            return json;
        }
    }

    public async Task<Result> SaveCompanyAsync(object model)
    {
        var database = Database;
        if (Config.App.IsPlatform)
        {
            var company = await database.QueryAsync<SysCompany>(d => d.Code == CurrentUser.CompNo);
            if (company == null)
                return Result.Error(Language["Tip.CompanyNotExists"]);

            company.CompanyData = Utils.ToJson(model);
            await database.SaveAsync(company);
        }
        else
        {
            await SystemService.SaveConfigAsync(database, KeyCompany, model);
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