using Microsoft.AspNetCore.Authorization;

namespace Known.Core.Controllers;

[Route("[controller]")]
public class SystemController : BaseController
{
    private SystemService Service => new(Context);

    [HttpGet("[action]")]
    public string GetConfig([FromQuery] string key) => Service.GetConfig(key);

    [HttpPost("[action]")]
    public Result SaveConfig([FromBody] ConfigInfo info) => Service.SaveConfig(info);

    [AllowAnonymous]
    [HttpPost("[action]")]
    public Result CheckInstall() => Service.CheckInstall();

    [AllowAnonymous]
    [HttpPost("[action]")]
    public Result UpdateKey([FromBody] InstallInfo info) => Service.UpdateKey(info);

    [AllowAnonymous]
    [HttpPost("[action]")]
    public Result SaveInstall([FromBody] InstallInfo info) => Service.SaveInstall(info);

    [HttpGet("[action]")]
    public SystemInfo GetSystem() => Service.GetSystem();

    [HttpPost("[action]")]
    public Result SaveSystem([FromBody] SystemInfo info) => Service.SaveSystem(info);
}