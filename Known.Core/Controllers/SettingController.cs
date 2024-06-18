namespace Known.Core.Controllers;

[ApiController]
public class SettingController : BaseController, ISettingService
{
    private readonly ISettingService service;

    public SettingController(ISettingService service)
    {
        service.Context = Context;
        this.service = service;
    }

    [HttpGet("GetUserSetting")]
    public Task<T> GetUserSettingAsync<T>(string bizType) => service.GetUserSettingAsync<T>(bizType);

    [HttpPost("DeleteUserSetting")]
    public Task<Result> DeleteUserSettingAsync(string bizType) => service.DeleteUserSettingAsync(bizType);

    [HttpPost("SaveUserSetting")]
    public Task<Result> SaveUserSettingAsync(string bizType, object bizData) => service.SaveUserSettingAsync(bizType, bizData);
}