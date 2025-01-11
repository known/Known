namespace Known.Services;

public partial interface IAdminService
{
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
    /// 异步保存组织架构信息。
    /// </summary>
    /// <param name="model">组织架构信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveOrganizationAsync(SysOrganization model);
}

partial class AdminService
{
    public Task<List<SysOrganization>> GetOrganizationsAsync()
    {
        var info = GetSystem();
        var lists = new List<SysOrganization>
        {
            new() { Id = info.CompNo, ParentId = "0", CompNo = info.CompNo, Code = info.CompNo, Name = info.CompName }
        };
        return Task.FromResult(lists);
    }

    public Task<Result> DeleteOrganizationsAsync(List<SysOrganization> models)
    {
        return Result.SuccessAsync("删除成功！");
    }

    public Task<Result> SaveOrganizationAsync(SysOrganization model)
    {
        return Result.SuccessAsync("保存成功！");
    }
}

partial class AdminClient
{
    public Task<List<SysOrganization>> GetOrganizationsAsync()
    {
        return Http.GetAsync<List<SysOrganization>>("/Admin/GetOrganizations");
    }

    public Task<Result> DeleteOrganizationsAsync(List<SysOrganization> models)
    {
        return Http.PostAsync("/Admin/DeleteOrganizations", models);
    }

    public Task<Result> SaveOrganizationAsync(SysOrganization model)
    {
        return Http.PostAsync("/Admin/SaveOrganization", model);
    }
}