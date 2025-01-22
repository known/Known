namespace Known.Services;

public partial interface IAdminService
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

partial class AdminService
{
    private const string KeyCompany = "Company";

    public Task<string> GetCompanyAsync()
    {
        var company = AppData.GetBizData<CompanyInfo>(KeyCompany);
        if (company == null)
        {
            var info = GetSystem();
            company = new CompanyInfo { Code = info.CompNo, Name = info.CompName };
        }
        var json = Utils.ToJson(company);
        return Task.FromResult(json);
    }

    public Task<Result> SaveCompanyAsync(object model)
    {
        AppData.SaveBizData(KeyCompany, model);
        return Result.SuccessAsync(Language.Success(Language.Save));
    }
}

partial class AdminClient
{
    public Task<string> GetCompanyAsync()
    {
        return Http.GetTextAsync("/Admin/GetCompany");
    }

    public Task<Result> SaveCompanyAsync(object model)
    {
        return Http.PostAsync("/Admin/SaveCompany", model);
    }
}