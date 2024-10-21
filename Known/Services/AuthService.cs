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
    /// <param name="token">用户Token。</param>
    /// <returns>注销结果。</returns>
    Task<Result> SignOutAsync(string token);

    /// <summary>
    /// 异步获取用户信息。
    /// </summary>
    /// <param name="userName">用户登录名。</param>
    /// <returns>用户信息。</returns>
    Task<UserInfo> GetUserAsync(string userName);

    /// <summary>
    /// 异步获取系统后台首页数据。
    /// </summary>
    /// <returns>后台首页数据。</returns>
    Task<AdminInfo> GetAdminAsync();

    /// <summary>
    /// 异步修改用户密码。
    /// </summary>
    /// <param name="info">修改密码对象。</param>
    /// <returns>修改结果。</returns>
    Task<Result> UpdatePasswordAsync(PwdFormInfo info);
}