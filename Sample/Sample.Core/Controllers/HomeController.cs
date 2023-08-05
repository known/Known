namespace Sample.Core.Controllers;

[Route("[controller]")]
public class HomeController : BaseController
{
    private HomeService Service => new(Context);

    [HttpGet("[action]")]
    public HomeInfo GetHome() => Service.GetHome();
}