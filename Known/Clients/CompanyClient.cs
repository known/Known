namespace Known.Clients;

public class CompanyClient : BaseClient
{
    public CompanyClient(Context context) : base(context) { }

    public async Task<T> GetCompanyAsync<T>()
    {
        var json = await Context.GetAsync("Company/GetCompany");
        return Utils.FromJson<T>(json);
    }

    public Task<Result> SaveCompanyAsync(object model) => Context.PostAsync("Company/SaveCompany", model);
}