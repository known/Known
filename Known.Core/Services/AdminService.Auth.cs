namespace Known.Services;

partial class AdminService
{
    [AllowAnonymous]
    public async Task<Result> RegisterAsync(RegisterFormInfo info)
    {
        if (info.Password != info.Password1)
            Result.Error(Language["Tip.PwdNotEqual"]);

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
            return Result.Error(Language["Tip.UserNameExists"]);

        user = new UserInfo
        {
            UserName = userName,
            Name = info.UserName,
            EnglishName = info.UserName,
            FirstLoginIP = info.IPAddress,
            Token = Utils.GetGuid()
        };
        database.User = await database.GetUserAsync(Constants.SysUserName);
        var result = await database.TransactionAsync("注册", async db =>
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
            await db.SaveAsync(model);
            await db.AddLogAsync(LogType.Register, user.UserName, $"IP：{user.LastLoginIP}");
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
        if (CoreConfig.OnLoging != null)
        {
            var vr = await CoreConfig.OnLoging.Invoke(database, info);
            if (!vr.IsValid)
                return vr;
        }

        var user = await database.GetUserInfoAsync(userName, password);
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

        var type = LogType.Login;
        if (info.IsMobile)
            type = LogType.AppLogin;

        database.User = user;
        var result = await database.TransactionAsync(Language["Login"], async db =>
        {
            await db.SaveUserAsync(user);
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

        return Result.Success(Language["Tip.ExitSuccess"]);
    }

    public async Task<AdminInfo> GetAdminAsync()
    {
        if (CurrentUser == null)
            return new AdminInfo();

        var db = Database;
        await db.OpenAsync();
        Config.System ??= await db.GetSystemAsync();
        var info = new AdminInfo
        {
            AppName = await db.GetUserSystemNameAsync(),
            DatabaseType = db.DatabaseType,
            UserMenus = await db.GetUserMenusAsync(),
            UserSetting = await db.GetUserSettingAsync<UserSettingInfo>(Constants.UserSetting),
            UserTableSettings = await db.GetUserTableSettingsAsync(),
            Codes = await db.GetDictionariesAsync(),
            Actions = AppData.GetActions()
        };
        info.UserSetting ??= CoreOption.Instance.UserSetting.Clone();
        if (CoreConfig.OnAdmin != null)
            await CoreConfig.OnAdmin.Invoke(db, info);
        await db.CloseAsync();
        Cache.AttachCodes(info.Codes);
        return info;
    }

    public async Task<UserInfo> GetUserAsync(string userName)
    {
        var user = await Database.QueryAsync<SysUser>(d => d.UserName == userName);
        return await Database.GetUserInfoAsync(user);
    }

    public async Task<UserInfo> GetUserByIdAsync(string userId)
    {
        var user = await Database.QueryAsync<SysUser>(d => d.Id == userId);
        return await Database.GetUserInfoAsync(user);
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
        return Result.Success(Language.SaveSuccess, url);
    }

    public async Task<Result> UpdateUserAsync(UserInfo info)
    {
        if (info == null)
            return Result.Error(Language["Tip.NoUser"]);

        var result = await Database.SaveUserAsync(info);
        if (!result.IsValid)
            return result;

        return Result.Success(Language.SaveSuccess, info);
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

        var oldPwd = Utils.ToMd5(info.OldPwd);
        if (entity.Password != oldPwd)
            return Result.Error(Language["Tip.CurPwdInvalid"]);

        entity.Password = Utils.ToMd5(info.NewPwd);
        await database.SaveAsync(entity);
        return Result.Success(Language.Success(Language["Button.Update"]), entity.Id);
    }
}