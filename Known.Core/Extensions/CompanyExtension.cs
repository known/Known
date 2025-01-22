namespace Known.Extensions;

static class CompanyExtension
{
    internal static async Task<string> GetCompanyDataAsync(this Database db, string compNo)
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

    internal static async Task<Result> SaveCompanyDataAsync(this Database db, string compNo, object model)
    {
        var lang = db.Context.Language;
        var data = await db.QueryAsync<SysCompany>(d => d.Code == compNo);
        if (data == null)
            return Result.Error(lang["Tip.CompanyNotExists"]);

        data.SystemData = Utils.ToJson(model);
        await db.SaveAsync(data);
        return Result.Success(lang.SaveSuccess);
    }
}