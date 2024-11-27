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
    /// 异步获取当前用户设置信息。
    /// </summary>
    /// <returns>用户设置信息。</returns>
    Task<UserSettingInfo> GetUserSettingAsync();

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

class AuthService(Context context) : ServiceBase(context), IAuthService
{
    public async Task<Result> SignInAsync(LoginFormInfo info)
    {
        var database = Database;
        var userName = info.UserName?.ToLower();
        await database.OpenAsync();
        var password = Utils.ToMd5(info.Password);
        var user = await database.GetUserInfoAsync(userName, password);
        await database.CloseAsync();
        if (user == null)
            return Result.Error(Language["Tip.LoginNoNamePwd"]);

        if (!user.Enabled)
            return Result.Error(Language["Tip.LoginDisabled"]);

        if (!user.FirstLoginTime.HasValue)
        {
            user.FirstLoginTime = DateTime.Now;
            user.FirstLoginIP = info.IPAddress;
        }
        user.LastLoginTime = DateTime.Now;
        user.LastLoginIP = info.IPAddress;
        user.Token = Utils.GetGuid();
        user.Station = info.Station;
        Cache.SetUser(user);

        var type = LogType.Login;
        if (info.IsMobile)
            type = LogType.AppLogin;

        database.User = user;
        return await database.TransactionAsync(Language["Login"], async db =>
        {
            await db.SaveUserAsync(user);
            await AddLogAsync(db, type, $"{user.UserName}-{user.Name}", $"IP：{user.LastLoginIP}");
        }, user);
    }

    public async Task<Result> SignOutAsync()
    {
        var user = CurrentUser;
        Cache.RemoveUser(user);
        if (user != null)
        {
            using var db = Database.Create();
            db.User = user;
            await AddLogAsync(db, LogType.Logout, $"{user.UserName}-{user.Name}", $"token: {user.Token}");
        }

        return Result.Success(Language["Tip.ExitSuccess"]);
    }

    public Task<UserSettingInfo> GetUserSettingAsync()
    {
        return Database.GetUserSettingAsync<UserSettingInfo>(Constants.UserSetting);
    }

    public async Task<AdminInfo> GetAdminAsync()
    {
        if (CurrentUser == null)
            return new AdminInfo();

        var db = Database;
        await db.OpenAsync();
        await db.CheckKeyAsync();
        var modules = await db.Query<SysModule>().ToListAsync<ModuleInfo>();
        DataHelper.Initialize(modules);
        var info = new AdminInfo
        {
            AppName = await db.GetSystemNameAsync(),
            UserMenus = await db.GetUserMenusAsync(modules),
            UserTableSettings = await db.GetUserTableSettingsAsync(),
            MessageCount = await db.CountAsync<SysMessage>(d => d.UserId == db.User.UserName && d.Status == Constant.UMStatusUnread),
            Codes = await DictionaryService.GetDictionariesAsync(db)
        };
        await db.CloseAsync();
        Cache.AttachCodes(info.Codes);
        return info;
    }

    public async Task<Result> UpdateAvatarAsync(AvatarInfo info)
    {
        var database = Database;
        var entity = await database.QueryAsync<SysUser>(d => d.Id == info.UserId);
        if (entity == null)
            return Result.Error(Language["Tip.NoUser"]);

        var attach = new AttachFile(info.File, CurrentUser);
        attach.FilePath = @$"Avatars\{entity.Id}{attach.ExtName}";
        await attach.SaveAsync();

        var url = Config.GetFileUrl(attach.FilePath);
        entity.SetExtension(nameof(UserInfo.AvatarUrl), url);
        await database.SaveAsync(entity);
        return Result.Success(Language.Success(Language.Save), url);
    }

    public async Task<Result> UpdateUserAsync(UserInfo info)
    {
        if (info == null)
            return Result.Error(Language["Tip.NoUser"]);

        var result = await Database.SaveUserAsync(info);
        if (!result.IsValid)
            return result;

        return Result.Success(Language.Success(Language.Save), info);
    }

    public async Task<Result> UpdatePasswordAsync(PwdFormInfo info)
    {
        var user = CurrentUser;
        if (user == null)
            return Result.Error(Language["Tip.NoLogin"]);

        var errors = new List<string>();
        if (string.IsNullOrEmpty(info.OldPwd))
            errors.Add(Language["Tip.CurPwdRequired"]);
        if (string.IsNullOrEmpty(info.NewPwd))
            errors.Add(Language["Tip.NewPwdRequired"]);
        if (string.IsNullOrEmpty(info.NewPwd1))
            errors.Add(Language["Tip.ConPwdRequired"]);
        if (info.NewPwd != info.NewPwd1)
            errors.Add(Language["Tip.PwdNotEqual"]);

        if (errors.Count > 0)
            return Result.Error(string.Join(Environment.NewLine, errors));

        var database = Database;
        var entity = await database.QueryByIdAsync<SysUser>(user.Id);
        if (entity == null)
            return Result.Error(Language["Tip.NoUser"]);

        info.OldPwd = Utils.ToMd5(info.NewPwd);
        if (entity.Password != info.OldPwd)
            return Result.Error(Language["Tip.CurPwdInvalid"]);

        entity.Password = Utils.ToMd5(info.NewPwd);
        await database.SaveAsync(entity);
        return Result.Success(Language.Success(Language["Button.Update"]), entity.Id);
    }

    private static Task AddLogAsync(Database db, LogType type, string target, string content)
    {
        return db.SaveAsync(new SysLog
        {
            Type = type.ToString(),
            Target = target,
            Content = content
        });
    }
}