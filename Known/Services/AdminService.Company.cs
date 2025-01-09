﻿namespace Known.Services;

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
    public Task<string> GetCompanyAsync()
    {
        return Task.FromResult("");
    }

    public Task<Result> SaveCompanyAsync(object model)
    {
        return Result.SuccessAsync("保存成功！");
    }
}

partial class AdminClient
{
    public Task<string> GetCompanyAsync()
    {
        return Http.GetStringAsync("/Admin/GetCompany");
    }

    public Task<Result> SaveCompanyAsync(object model)
    {
        return Http.PostAsync("/Admin/SaveCompany", model);
    }
}