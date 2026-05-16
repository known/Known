namespace Known.Extensions;

/// <summary>
/// 租户数据扩展类。
/// </summary>
public static class CompanyExtension
{
    /// <summary>
    /// 异步获取租户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="compNo">租户编码。</param>
    /// <returns></returns>
    public static async Task<string> GetCompanyDataAsync(this Database db, string compNo)
    {
        var model = await db.QueryAsync<SysCompany>(d => d.Code == compNo);
        if (model == null)
            return string.Empty;

        var data = model.CompanyData;
        if (string.IsNullOrWhiteSpace(data))
        {
            data = Utils.ToJson(new
            {
                model.Code,
                model.Name,
                model.NameEn,
                model.SccNo,
                model.Address,
                model.AddressEn
            });
        }
        return data;
    }

    /// <summary>
    /// 异步保存租户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="compNo">租户编码。</param>
    /// <param name="model">租户信息对象。</param>
    /// <returns></returns>
    public static async Task<Result> SaveCompanyDataAsync(this Database db, string compNo, object model)
    {
        var data = await db.QueryAsync<SysCompany>(d => d.Code == compNo);
        if (data == null)
            return Result.Error(Language.TipCompanyNotExists);

        data.CompanyData = Utils.ToJson(model);
        await db.SaveAsync(data);
        return Result.Success(Language.SaveSuccess);
    }

    internal static async Task InitializeTenantsAsync(this Database db)
    {
        if (!Config.App.IsPlatform)
            return;

        var tenants = await db.QueryListAsync<SysCompany>();
        if (tenants == null || tenants.Count == 0)
            return;

        var dbTenants = tenants.Where(d => !string.IsNullOrWhiteSpace(d.SystemData?.ConnString)).ToList();
        if (dbTenants == null || dbTenants.Count == 0)
            return;

        foreach (var item in dbTenants)
        {
            CoreConfig.SystemInfos[item.Code] = item.SystemData;
        }
    }
}