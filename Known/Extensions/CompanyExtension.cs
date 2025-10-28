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

        data.SystemData = Utils.ToJson(model);
        await db.SaveAsync(data);
        return Result.Success(Language.SaveSuccess);
    }
}