namespace Sample.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<string> Get()
    {
        var db = Database.Create();
        var user = await db.Query<SysUser>().Where(d => d.UserName == "admin").FirstAsync();
        return user?.Name;
    }
}