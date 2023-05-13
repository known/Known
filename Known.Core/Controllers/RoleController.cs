namespace Known.Core.Controllers;

[Route("[controller]")]
public class RoleController : BaseController
{
    private RoleService Service => new(Context);

    [HttpPost("[action]")]
    public PagingResult<SysRole> QueryRoles([FromBody] PagingCriteria criteria) => Service.QueryRoles(criteria);

    [HttpPost("[action]")]
    public Result DeleteRoles([FromBody] List<SysRole> models) => Service.DeleteRoles(models);

    [HttpGet("[action]")]
    public RoleFormInfo GetRole([FromQuery] string roleId) => Service.GetRole(roleId);

    [HttpPost("[action]")]
    public Result SaveRole([FromBody] RoleFormInfo info)
    {
        info.Model = GetDynamicModel(info.Model);
        return Service.SaveRole(info);
    }
}