namespace Known.Services;

/// <summary>
/// 身份认证服务接口。
/// </summary>
public interface IAuthService : IService
{
    /// <summary>
    /// 异步用户登录。
    /// </summary>
    /// <param name="info">登录表单对象。</param>
    /// <returns>登录结果。</returns>
    [AllowAnonymous] Task<Result> SignInAsync(LoginFormInfo info);

    /// <summary>
    /// 异步注销登录。
    /// </summary>
    /// <returns>注销结果。</returns>
    Task<Result> SignOutAsync();

    /// <summary>
    /// 异步获取系统后台首页数据。
    /// </summary>
    /// <returns>后台首页数据。</returns>
    Task<AdminInfo> GetAdminAsync();

    /// <summary>
    /// 异步修改系统用户头像。
    /// </summary>
    /// <param name="info">用户头像信息。</param>
    /// <returns>修改结果。</returns>
    Task<Result> UpdateAvatarAsync(AvatarInfo info);

    /// <summary>
    /// 异步修改系统用户信息。
    /// </summary>
    /// <param name="info">系统用户信息。</param>
    /// <returns>修改结果。</returns>
    Task<Result> UpdateUserAsync(UserInfo info);

    /// <summary>
    /// 异步修改用户密码。
    /// </summary>
    /// <param name="info">修改密码对象。</param>
    /// <returns>修改结果。</returns>
    Task<Result> UpdatePasswordAsync(PwdFormInfo info);
}

class AuthService(Context context, INodbProvider provider) : ServiceBase(context), IAuthService
{
    public Task<Result> SignInAsync(LoginFormInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SignOutAsync()
    {
        return Result.SuccessAsync("");
    }

    public Task<UserInfo> GetUserAsync(string userName)
    {
        return Task.FromResult(CurrentUser);
    }

    public async Task<AdminInfo> GetAdminAsync()
    {
        var info = new AdminInfo
        {
            AppName = App.Name,
            UserMenus = await provider.GetUserMenusAsync(CurrentUser),
            Codes = await provider.GetCodesAsync(CurrentUser)
        };
        return info;
    }

    public Task<Result> UpdateAvatarAsync(AvatarInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateUserAsync(UserInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdatePasswordAsync(PwdFormInfo info)
    {
        throw new NotImplementedException();
    }
}