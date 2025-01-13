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
    public Task<List<OrganizationInfo>> GetOrganizationsAsync()
    {
        var info = GetSystem();
        var lists = new List<OrganizationInfo>
        {
            new() { Id = info.CompNo, ParentId = "0", Code = info.CompNo, Name = info.CompName }
        };
        return Task.FromResult(lists);
    }

    public Task<Result> DeleteOrganizationsAsync(List<OrganizationInfo> infos)
    {
        return Result.SuccessAsync("删除成功！");
    }

    public Task<Result> SaveOrganizationAsync(OrganizationInfo info)
    {
        return Result.SuccessAsync("保存成功！");
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