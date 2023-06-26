using Microsoft.AspNetCore.Authorization;

namespace Known.Core.Controllers;

[Route("[controller]")]
public class SystemController : BaseController
{
    private SystemService Service => new(Context);

    //Config
    [HttpGet("[action]")]
    public string GetConfig([FromQuery] string key) => Service.GetConfig(key);

    [HttpPost("[action]")]
    public Result SaveConfig([FromBody] ConfigInfo info) => Service.SaveConfig(info);

    //Install
    [AllowAnonymous]
    [HttpPost("[action]")]
    public Result CheckInstall() => Service.CheckInstall();

    [AllowAnonymous]
    [HttpPost("[action]")]
    public Result UpdateKey([FromBody] InstallInfo info) => Service.UpdateKey(info);

    [AllowAnonymous]
    [HttpPost("[action]")]
    public Result SaveInstall([FromBody] InstallInfo info) => Service.SaveInstall(info);

    //System
    [HttpGet("[action]")]
    public SystemInfo GetSystem() => Service.GetSystem();

    [HttpPost("[action]")]
    public Result SaveSystem([FromBody] SystemInfo info) => Service.SaveSystem(info);

    //Tenant
    [HttpPost("[action]")]
    public PagingResult<SysTenant> QueryTenants([FromBody] PagingCriteria criteria) => Service.QueryTenants(criteria);

    [HttpPost("[action]")]
    public Result SaveTenant([FromBody] object model) => Service.SaveTenant(GetDynamicModel(model));

}