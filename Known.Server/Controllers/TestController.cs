using Microsoft.AspNetCore.Mvc;

namespace Known.Server.Controllers;

[Anonymous]
[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet]
    public string Index()
    {
        return "Api is running.";
    }

    [HttpGet("GetName")]
    public string GetName([FromQuery] string name)
    {
        return $"Hi {name}";
    }

    [HttpPost("SaveUser")]
    public Result SaveUser([FromBody] UserInfo info)
    {
        return Result.Success($"{info.UserName}登录成功！");
    }
}