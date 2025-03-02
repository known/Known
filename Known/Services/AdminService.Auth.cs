namespace Known.Services;

public partial interface IAdminService
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
    /// 异步获取用户信息。
    /// </summary>
    /// <param name="userName">用户名。</param>
    /// <returns>用户信息。</returns>
    Task<UserInfo> GetUserAsync(string userName);

    /// <summary>
    /// 异步根据ID获取用户信息。
    /// </summary>
    /// <param name="userId">用户ID。</param>
    /// <returns>用户信息。</returns>
    Task<UserInfo> GetUserByIdAsync(string userId);

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

partial class AdminService
{
    public Task<Result> SignInAsync(LoginFormInfo info)
    {
        if (string.IsNullOrWhiteSpace(info.UserName))
            return Result.ErrorAsync("用户名不能为空！");

        var user = new UserInfo
        {
            UserName = info.UserName,
            Name = "管理员",
            AvatarUrl = "img/face1.png",
            Token = Utils.GetGuid(),
            Role = "管理员"
        };
        Cache.SetUser(user);
        return Result.SuccessAsync(Language.Success(Language["Login"]), user);
    }

    public Task<Result> SignOutAsync()
    {
        Cache.RemoveUser(CurrentUser);
        return Result.SuccessAsync(Language["Tip.ExitSuccess"]);
    }

    public Task<AdminInfo> GetAdminAsync()
    {
        var modules = DataHelper.GetModules(AppData.Data.Modules);
        var info = new AdminInfo
        {
            AppName = Config.App.Name,
            UserMenus = modules.ToMenus()
        };
        return Task.FromResult(info);
    }

    public Task<UserInfo> GetUserAsync(string userName)
    {
        var user = Cache.GetUser(userName);
        return Task.FromResult(user);
    }

    public Task<UserInfo> GetUserByIdAsync(string userId)
    {
        return Task.FromResult(new UserInfo { Id = userId });
    }

    public Task<Result> UpdateAvatarAsync(AvatarInfo info)
    {
        return Result.SuccessAsync(Language.SaveSuccess);
    }

    public Task<Result> UpdateUserAsync(UserInfo info)
    {
        return Result.SuccessAsync(Language.SaveSuccess);
    }

    public Task<Result> UpdatePasswordAsync(PwdFormInfo info)
    {
        return Result.SuccessAsync(Language.SaveSuccess);
    }
}

partial class AdminClient
{
    public Task<Result> SignInAsync(LoginFormInfo info)
    {
        return Http.PostAsync("/Admin/SignIn", info);
    }

    public Task<Result> SignOutAsync()
    {
        return Http.PostAsync("/Admin/SignOut");
    }

    public Task<AdminInfo> GetAdminAsync()
    {
        return Http.GetAsync<AdminInfo>("/Admin/GetAdmin");
    }

    public Task<UserInfo> GetUserAsync(string userName)
    {
        return Http.GetAsync<UserInfo>($"/Admin/GetUser?userName={userName}");
    }

    public Task<UserInfo> GetUserByIdAsync(string userId)
    {
        return Http.GetAsync<UserInfo>($"/Admin/GetUserById?userId={userId}");
    }

    public Task<Result> UpdateAvatarAsync(AvatarInfo info)
    {
        return Http.PostAsync("/Admin/UpdateAvatar", info);
    }

    public Task<Result> UpdateUserAsync(UserInfo info)
    {
        return Http.PostAsync("/Admin/UpdateUser", info);
    }

    public Task<Result> UpdatePasswordAsync(PwdFormInfo info)
    {
        return Http.PostAsync("/Admin/UpdatePassword", info);
    }
}