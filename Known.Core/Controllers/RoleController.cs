namespace Known.Core.Controllers;

[ApiController]
public class RoleController : BaseController, IRoleService
{
    private readonly IRoleService service;

    public RoleController(IRoleService service)
    {
        service.Context = Context;
        this.service = service;
    }

    [HttpPost("QueryRoles")]
    public Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria) => service.QueryRolesAsync(criteria);

    [HttpGet("GetRole")]
    public Task<SysRole> GetRoleAsync(string roleId) => service.GetRoleAsync(roleId);

    [HttpPost("DeleteRoles")]
    public Task<Result> DeleteRolesAsync(List<SysRole> models) => service.DeleteRolesAsync(models);

    [HttpPost("SaveRole")]
    public Task<Result> SaveRoleAsync(SysRole model) => service.SaveRoleAsync(model);
}