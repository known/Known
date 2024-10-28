namespace Known.Core;

/// <summary>
/// 认证控制器类，用于登录。
/// </summary>
/// <param name="service">认证服务。</param>
public class AuthController(IAuthService service) : ControllerBase
{
    /// <summary>
    /// 用户登录。
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    [Route("signin")]
    public async Task<IResult> LoginAsync([FromBody] LoginFormInfo info)
    {
        var result = await service.SignInAsync(info);
        if (!result.IsValid)
            return Results.Ok(result);

        var user = result.DataAs<UserInfo>();
        var principal = user.ToPrincipal(Constants.KeyAuth);
        await HttpContext.SignInAsync(Constants.KeyAuth, principal);
        return Results.Redirect("/");
    }

    /// <summary>
    /// 用户退出。
    /// </summary>
    /// <returns></returns>
    [Route("signout")]
    public Task LogoutAsync()
    {
        return HttpContext.SignOutAsync(Constants.KeyAuth);
    }
}