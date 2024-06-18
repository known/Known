namespace Known.Core.Controllers;

[ApiController]
public class UserController : BaseController, IUserService
{
    private readonly IUserService service;

    public UserController(IUserService service)
    {
        service.Context = Context;
        this.service = service;
    }

    [HttpPost("QueryUsers")]
    public Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria) => service.QueryUsersAsync(criteria);

    [HttpGet("GetUser")]
    public Task<SysUser> GetUserAsync(string id) => service.GetUserAsync(id);

    [HttpGet("GetUserData")]
    public Task<SysUser> GetUserDataAsync(string id) => service.GetUserDataAsync(id);

    [HttpPost("DeleteUsers")]
    public Task<Result> DeleteUsersAsync(List<SysUser> models) => service.DeleteUsersAsync(models);

    [HttpPost("ChangeDepartment")]
    public Task<Result> ChangeDepartmentAsync(List<SysUser> models) => service.ChangeDepartmentAsync(models);

    [HttpPost("EnableUsers")]
    public Task<Result> EnableUsersAsync(List<SysUser> models) => service.EnableUsersAsync(models);

    [HttpPost("DisableUsers")]
    public Task<Result> DisableUsersAsync(List<SysUser> models) => service.DisableUsersAsync(models);

    [HttpPost("SetUserPwds")]
    public Task<Result> SetUserPwdsAsync(List<SysUser> models) => service.SetUserPwdsAsync(models);

    [HttpPost("UpdateUser")]
    public Task<Result> UpdateUserAsync(SysUser model) => service.UpdateUserAsync(model);

    [HttpPost("SaveUser")]
    public Task<Result> SaveUserAsync(SysUser model) => service.SaveUserAsync(model);
}