namespace Known.Clients;

public class CompanyClient : ClientBase
{
    public CompanyClient(Context context) : base(context) { }

    //Company
    public async Task<T> GetCompanyAsync<T>()
    {
        var json = await Context.GetAsync("Company/GetCompany");
        return Utils.FromJson<T>(json);
    }

    public Task<Result> SaveCompanyAsync(object model) => Context.PostAsync("Company/SaveCompany", model);

    //Organization
    public Task<List<SysOrganization>> GetOrganizationsAsync() => Context.GetAsync<List<SysOrganization>>("Company/GetOrganizations");
    public Task<Result> DeleteOrganizationsAsync(List<SysOrganization> models) => Context.PostAsync("Company/DeleteOrganizations", models);
    public Task<Result> SaveOrganizationAsync(object model) => Context.PostAsync("Company/SaveOrganization", model);
}