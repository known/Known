using Microsoft.AspNetCore.Authorization;

namespace Known.Core.Controllers;

[Route("[controller]")]
public class UserController : BaseController
{
    private UserService Service => new(Context);

    //User
    [HttpPost("[action]")]
    public PagingResult<SysUser> QueryUsers([FromBody] PagingCriteria criteria) => Service.QueryUsers(criteria);

    [HttpPost("[action]")]
    public Result DeleteUsers([FromBody] List<SysUser> models) => Service.DeleteUsers(models);

    [HttpPost("[action]")]
    public Result SetUserPwds([FromBody] List<SysUser> models) => Service.SetUserPwds(models);

    [HttpPost("[action]")]
    public Result SaveUser([FromBody] object model) => Service.SaveUser(GetDynamicModel(model));

    [HttpPost("[action]")]
    public Result UpdateUser([FromBody] object model) => Service.UpdateUser(GetDynamicModel(model));

    [HttpGet("[action]")]
    public UserAuthInfo GetUserAuth([FromQuery] string userId) => Service.GetUserAuth(userId);

    //Account
    [AllowAnonymous]
    [HttpPost("[action]")]
    public Result SignIn([FromBody] LoginFormInfo info)
    {
        info.IPAddress = Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        return Service.SignIn(info);
    }

    [HttpPost("[action]/{token}")]
    public Result SignOut(string token) => Service.SignOut(token);

    [HttpGet("[action]")]
    public AdminInfo GetAdmin() => Service.GetAdmin();

    [HttpPost("[action]")]
    public Result UpdatePassword([FromBody] PwdFormInfo info) => Service.UpdatePassword(info);

    [HttpPost("[action]")]
    public Result DeleteSetting([FromBody] SettingFormInfo info) => Service.DeleteSetting(info);
    
    [HttpPost("[action]")]
    public Result SaveSetting([FromBody] SettingFormInfo info) => Service.SaveSetting(info);

    //Message
    [HttpPost("[action]")]
    public PagingResult<SysMessage> QueryMessages([FromBody] PagingCriteria criteria) => Service.QueryMessages(criteria);

    [HttpPost("[action]")]
    public Result DeleteMessages([FromBody] List<SysMessage> models) => Service.DeleteMessages(models);

    [HttpPost("[action]")]
    public Result SaveMessage([FromBody] object model) => Service.SaveMessage(GetDynamicModel(model));
}