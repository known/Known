namespace Known.Core.Controllers;

[Route("[controller]")]
public class CompanyController : BaseController
{
    private CompanyService Service => new(Context);

    [HttpGet("[action]")]
    public string GetCompany() => Service.GetCompany();

    [HttpPost("[action]")]
    public Result SaveCompany([FromBody] object model) => Service.SaveCompany(model);
}