namespace Known.Services;

/// <summary>
/// 系统租户服务接口。
/// </summary>
public interface ICompanyService : IService
{
    /// <summary>
    /// 异步获取租户企业信息JSON。
    /// </summary>
    /// <returns>企业信息JSON。</returns>
    Task<string> GetCompanyAsync();

    /// <summary>
    /// 异步保存租户企业信息。
    /// </summary>
    /// <param name="model">企业信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveCompanyAsync(object model);
}

class CompanyService(Context context) : ServiceBase(context), ICompanyService
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
            var json = await Admin.GetConfigAsync(database, KeyCompany);
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
            await Admin.SaveConfigAsync(database, KeyCompany, model);
        }
        return Result.Success(Language.Success(Language.Save));
    }

    private async Task<string> GetCompanyDataAsync(Database db)
    {
        var data = await Admin.GetCompanyDataAsync(db, db.User.CompNo);
        if (!string.IsNullOrWhiteSpace(data))
            return data;

        return GetDefaultData(db.User);
    }

    private static string GetDefaultData(UserInfo user)
    {
        return Utils.ToJson(new { Code = user.CompNo, Name = user.CompName });
    }
}