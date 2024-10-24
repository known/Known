namespace Known.Services;

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
    public Task<string> GetCompanyAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<SysOrganization>> GetOrganizationsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteOrganizationsAsync(List<SysOrganization> models)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveCompanyAsync(object model)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveOrganizationAsync(SysOrganization model)
    {
        throw new NotImplementedException();
    }
}