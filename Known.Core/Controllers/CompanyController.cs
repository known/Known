namespace Known.Core.Controllers;

[ApiController]
public class CompanyController : BaseController, ICompanyService
{
    private readonly ICompanyService service;

    public CompanyController(ICompanyService service)
    {
        service.Context = Context;
        this.service = service;
    }

    [HttpGet("GetCompany")]
    public Task<T> GetCompanyAsync<T>() => service.GetCompanyAsync<T>();

    [HttpGet("GetOrganizations")]
    public Task<List<SysOrganization>> GetOrganizationsAsync() => service.GetOrganizationsAsync();

    [HttpPost("DeleteOrganizations")]
    public Task<Result> DeleteOrganizationsAsync(List<SysOrganization> models) => service.DeleteOrganizationsAsync(models);

    [HttpPost("SaveCompany")]
    public Task<Result> SaveCompanyAsync(object model) => service.SaveCompanyAsync(model);

    [HttpPost("SaveOrganization")]
    public Task<Result> SaveOrganizationAsync(SysOrganization model) => service.SaveOrganizationAsync(model);
}