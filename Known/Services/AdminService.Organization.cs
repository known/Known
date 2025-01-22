namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步获取组织架构列表。
    /// </summary>
    /// <returns>组织架构列表。</returns>
    Task<List<OrganizationInfo>> GetOrganizationsAsync();

    /// <summary>
    /// 异步删除组织架构信息。
    /// </summary>
    /// <param name="infos">组织架构列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteOrganizationsAsync(List<OrganizationInfo> infos);

    /// <summary>
    /// 异步保存组织架构信息。
    /// </summary>
    /// <param name="info">组织架构信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveOrganizationAsync(OrganizationInfo info);
}

partial class AdminService
{
    private const string KeyOrganization = "Organizations";

    public Task<List<OrganizationInfo>> GetOrganizationsAsync()
    {
        var datas = AppData.GetBizData<List<OrganizationInfo>>(KeyOrganization) ?? [];
        var info = GetSystem();
        datas.Add(new() { Id = info.CompNo, ParentId = "0", Code = info.CompNo, Name = info.CompName });
        return Task.FromResult(datas);
    }

    public Task<Result> DeleteOrganizationsAsync(List<OrganizationInfo> infos)
    {
        return DeleteModelsAsync(KeyOrganization, infos);
    }

    public Task<Result> SaveOrganizationAsync(OrganizationInfo info)
    {
        return SaveModelAsync(KeyOrganization, info);
    }
}

partial class AdminClient
{
    public Task<List<OrganizationInfo>> GetOrganizationsAsync()
    {
        return Http.GetAsync<List<OrganizationInfo>>("/Admin/GetOrganizations");
    }

    public Task<Result> DeleteOrganizationsAsync(List<OrganizationInfo> infos)
    {
        return Http.PostAsync("/Admin/DeleteOrganizations", infos);
    }

    public Task<Result> SaveOrganizationAsync(OrganizationInfo info)
    {
        return Http.PostAsync("/Admin/SaveOrganization", info);
    }
}