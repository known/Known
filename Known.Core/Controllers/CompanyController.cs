namespace Known.Core.Controllers;

[Route("[controller]")]
public class CompanyController : BaseController
{
    private CompanyService Service => new(Context);

    //Company
    [HttpGet("[action]")]
    public string GetCompany() => Service.GetCompany();

    [HttpPost("[action]")]
    public Result SaveCompany([FromBody] object model) => Service.SaveCompany(model);

    //Organization
    [HttpGet("[action]")]
    public List<SysOrganization> GetOrganizations() => Service.GetOrganizations();

    [HttpPost("[action]")]
    public Result DeleteOrganizations([FromBody] List<SysOrganization> models) => Service.DeleteOrganizations(models);

    [HttpPost("[action]")]
    public Result SaveOrganization([FromBody] object model) => Service.SaveOrganization(GetDynamicModel(model));
}