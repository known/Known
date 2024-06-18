namespace Known.Core.Controllers;

//[Route("[controller]")]
[ApiController]
public class AuthController : BaseController, IAuthService
{
    private readonly IAuthService service;

    public AuthController(IAuthService service)
    {
        service.Context = Context;
        this.service = service;
    }

    [HttpPost("SignIn")]
    public Task<Result> SignInAsync(LoginFormInfo info) => service.SignInAsync(info);
    
    [HttpPost("SignOut")]
    public Task<Result> SignOutAsync(string token) => service.SignOutAsync(token);
    
    [HttpGet("GetUser")]
    public Task<UserInfo> GetUserAsync(string userName) => service.GetUserAsync(userName);
    
    [HttpGet("GetAdmin")]
    public Task<AdminInfo> GetAdminAsync() => service.GetAdminAsync();
    
    [HttpPost("UpdatePassword")]
    public Task<Result> UpdatePasswordAsync(PwdFormInfo info) => service.UpdatePasswordAsync(info);
}