namespace Known.Services;

partial class AdminService
{
    private const string KeyCompany = "CompanyInfo";

    public async Task<string> GetCompanyAsync()
    {
        var database = Database;
        if (Config.App.IsPlatform)
        {
            return await GetCompanyDataAsync(database);
        }
        else
        {
            var json = await database.GetConfigAsync(KeyCompany);
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
            var result = await database.SaveCompanyDataAsync(CurrentUser.CompNo, model);
            if (!result.IsValid)
                return result;
        }
        else
        {
            await database.SaveConfigAsync(KeyCompany, model);
        }
        return Result.Success(Language.SaveSuccess);
    }

    private static async Task<string> GetCompanyDataAsync(Database db)
    {
        var data = await db.GetCompanyDataAsync(db.User.CompNo);
        if (!string.IsNullOrWhiteSpace(data))
            return data;

        return GetDefaultData(db.User);
    }

    private static string GetDefaultData(UserInfo user)
    {
        return Utils.ToJson(new { Code = user.CompNo, Name = user.CompName });
    }
}