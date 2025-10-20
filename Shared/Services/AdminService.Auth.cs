namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步注册用户。
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    [AllowAnonymous] Task<Result> RegisterAsync(RegisterFormInfo info);

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

partial class AdminClient
{
    public Task<Result> RegisterAsync(RegisterFormInfo info) => Http.PostAsync("/Admin/Register", info);
    public Task<Result> SignInAsync(LoginFormInfo info) => Http.PostAsync("/Admin/SignIn", info);
    public Task<Result> SignOutAsync() => Http.PostAsync("/Admin/SignOut");
    public Task<AdminInfo> GetAdminAsync() => Http.GetAsync<AdminInfo>("/Admin/GetAdmin");
    public Task<UserInfo> GetUserAsync(string userName) => Http.GetAsync<UserInfo>($"/Admin/GetUser?userName={userName}");
    public Task<UserInfo> GetUserByIdAsync(string userId) => Http.GetAsync<UserInfo>($"/Admin/GetUserById?userId={userId}");
    public Task<Result> UpdateAvatarAsync(AvatarInfo info) => Http.PostAsync("/Admin/UpdateAvatar", info);
    public Task<Result> UpdateUserAsync(UserInfo info) => Http.PostAsync("/Admin/UpdateUser", info);
    public Task<Result> UpdatePasswordAsync(PwdFormInfo info) => Http.PostAsync("/Admin/UpdatePassword", info);
}

partial class AdminService
{
    [AllowAnonymous]
    public async Task<Result> RegisterAsync(RegisterFormInfo info)
    {
        if (info.Password != info.Password1)
            return Result.Error(Language.TipPwdNotEqual);

        var database = Database;
        if (CoreConfig.OnRegistering != null)
        {
            var vr = await CoreConfig.OnRegistering.Invoke(database, info);
            if (!vr.IsValid)
                return vr;
        }

        var userName = info.UserName?.ToLower();
        var user = await database.GetUserAsync(userName);
        if (user != null)
            return Result.Error(Language.TipUserNameExists);

        user = new UserInfo
        {
            UserName = userName,
            Name = info.UserName,
            EnglishName = info.UserName,
            FirstLoginIP = info.IPAddress,
            Token = Utils.GetGuid()
        };
        database.User = await database.GetUserAsync(Constants.SysUserName);
        var result = await database.TransactionAsync(Language.Register, async db =>
        {
            var model = new SysUser
            {
                UserName = info.UserName.ToLower(),
                Name = info.UserName,
                EnglishName = info.UserName,
                Password = Utils.ToMd5(info.Password),
                FirstLoginTime = DateTime.Now,
                FirstLoginIP = info.IPAddress,
                LastLoginTime = DateTime.Now,
                LastLoginIP = info.IPAddress
            };
            if (CoreConfig.OnRegistered != null)
                await CoreConfig.OnRegistered.Invoke(db, model);
            await db.AddUserAsync(model);
            await db.AddLogAsync(LogType.Register, user.UserName, $"IP：{user.LastLoginIP}");
            user.Id = model.Id;
            user.AppId = model.AppId;
            user.CompNo = model.CompNo;
        }, user);
        if (result.IsValid)
            Cache.SetUser(user);
        return result;
    }

    [AllowAnonymous]
    public async Task<Result> SignInAsync(LoginFormInfo info)
    {
        var database = Database;
        var userName = info.UserName?.ToLower();
        var password = Utils.ToMd5(info.Password);
        var cacheUser = Cache.GetUser(userName);
        if (Constants.SysUserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && 
            CoreConfig.DevRoles.TryGetValue(password, out string role))
        {
            var admin = await database.GetUserAsync(userName);
            admin.Role = role;
            admin.Token = cacheUser != null ? cacheUser.Token : Utils.GetGuid();
            Cache.SetUser(admin);
            return Result.Success(Language.Success(Language.Login), admin);
        }

        if (CoreConfig.OnLoging != null)
        {
            var vr = await CoreConfig.OnLoging.Invoke(database, info);
            if (!vr.IsValid)
                return vr;
        }

        var user = await database.GetUserAsync(userName, password);
        if (user == null)
            return Result.Error(Language.TipLoginNoNamePwd);

        if (!user.Enabled)
            return Result.Error(Language.TipLoginDisabled);

        if (!user.FirstLoginTime.HasValue)
        {
            user.FirstLoginTime = DateTime.Now;
            user.FirstLoginIP = info.IPAddress;
        }
        user.LastLoginTime = DateTime.Now;
        user.LastLoginIP = info.IPAddress;
        user.Station = info.Station;
        user.Token = cacheUser != null ? cacheUser.Token : Utils.GetGuid();

        var type = LogType.Login;
        if (info.IsMobile)
            type = LogType.AppLogin;

        database.User = user;
        var result = await database.TransactionAsync(Language.Login, async db =>
        {
            if (CoreConfig.OnLoged != null)
                await CoreConfig.OnLoged.Invoke(db, user);
            await db.SaveUserAsync(Context, user);
            await db.AddLogAsync(type, $"{user.UserName}-{user.Name}", $"IP：{user.LastLoginIP}");
        }, user);
        if (result.IsValid)
            Cache.SetUser(user);
        return result;
    }

    public async Task<Result> SignOutAsync()
    {
        var user = CurrentUser;
        Cache.RemoveUser(user);
        if (user != null)
        {
            using var db = Database.Create();
            db.User = user;
            await db.AddLogAsync(LogType.Logout, $"{user.UserName}-{user.Name}", $"token: {user.Token}");
        }

        return Result.Success(Language.ExitSuccess);
    }

    public async Task<AdminInfo> GetAdminAsync()
    {
        var info = new AdminInfo();
        if (CurrentUser == null)
            return info;

        CoreConfig.Load(info);
        await Database.QueryActionAsync(async db =>
        {
            CoreConfig.System = await db.GetSystemAsync();
            info.Actions = await db.GetActionsAsync();
            info.AppName = CurrentUser.AppName; //await db.GetUserSystemNameAsync();
            info.IsChangePwd = await db.CheckUserDefaultPasswordAsync(CoreConfig.System);
            info.DatabaseType = db.DatabaseType;
            info.UserMenus = await db.GetUserMenusAsync();
            info.UserSetting = await db.GetUserSettingAsync<UserSettingInfo>(Constants.UserSetting);
            info.UserTableSettings = await db.GetUserTableSettingsAsync();
            info.Codes = await db.GetDictionariesAsync();
            if (CoreConfig.OnAdmin != null)
                await CoreConfig.OnAdmin.Invoke(db, info);
        });
        info.UserSetting ??= CoreConfig.UserSetting.Clone();
        Cache.AttachCodes(info.Codes);
        return info;
    }

    public Task<UserInfo> GetUserAsync(string userName)
    {
        return Database.GetUserAsync(userName);
    }

    public Task<UserInfo> GetUserByIdAsync(string userId)
    {
        return Database.GetUserByIdAsync(userId);
    }

    public Task<Result> UpdateAvatarAsync(AvatarInfo info)
    {
        return Database.UpdateAvatarAsync(info);
    }

    public async Task<Result> UpdateUserAsync(UserInfo info)
    {
        if (info == null)
            return Result.Error(Language.TipNoUser);

        var result = await Database.SaveUserAsync(Context, info);
        if (!result.IsValid)
            return result;

        return Result.Success(Language.SaveSuccess, info);
    }

    public async Task<Result> UpdatePasswordAsync(PwdFormInfo info)
    {
        var user = CurrentUser;
        if (user == null)
            return Result.Error(Language.TipNoLogin);

        var errors = new List<string>();
        if (string.IsNullOrEmpty(info.OldPwd))
            errors.Add(Language[Language.TipCurPwdRequired]);
        if (string.IsNullOrEmpty(info.NewPwd))
            errors.Add(Language[Language.TipNewPwdRequired]);
        if (string.IsNullOrEmpty(info.NewPwd1))
            errors.Add(Language[Language.TipConPwdRequired]);
        if (info.NewPwd != info.NewPwd1)
            errors.Add(Language[Language.TipPwdNotEqual]);

        if (errors.Count > 0)
            return Result.Error(string.Join(Environment.NewLine, errors));

        var database = Database;
        var sys = await database.GetSystemAsync();
        var validator = new PasswordValidator();
        validator.Validate(sys, info.NewPwd, true);
        if (!string.IsNullOrWhiteSpace(validator.Message))
            return Result.Error($"{validator.Message}");

        info.UserId = user.Id;
        return await database.UpdatePasswordAsync(info);
    }
}