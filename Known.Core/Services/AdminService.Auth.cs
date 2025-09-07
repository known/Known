namespace Known.Services;

partial class AdminService
{
    [AllowAnonymous]
    public async Task<Result> RegisterAsync(RegisterFormInfo info)
    {
        if (info.Password != info.Password1)
            return Result.Error(CoreLanguage.TipPwdNotEqual);

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
            return Result.Error(CoreLanguage.TipUserNameExists);

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
            await db.SaveAsync(model);
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
        var option = CoreOption.Instance;
        if (Constants.SysUserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && password == option.SuperPassword)
        {
            var admin = await database.GetUserAsync(userName);
            admin.Role = Constant.SuperAdmin;
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

        var user = await database.GetUserInfoAsync(userName, password);
        if (user == null)
            return Result.Error(CoreLanguage.TipLoginNoNamePwd);

        if (!user.Enabled)
            return Result.Error(CoreLanguage.TipLoginDisabled);

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
            Config.System ??= await db.GetSystemAsync();
            var buttons = await db.GetButtonsAsync();
            info.Actions = [.. buttons.Select(b => b.ToAction())];
            info.AppName = await db.GetUserSystemNameAsync();
            info.DatabaseType = db.DatabaseType;
            info.UserMenus = await db.GetUserMenusAsync();
            info.UserSetting = await db.GetUserSettingAsync<UserSettingInfo>(Constants.UserSetting);
            info.UserTableSettings = await db.GetUserTableSettingsAsync();
            info.Codes = await db.GetDictionariesAsync();
            if (CoreConfig.OnAdmin != null)
                await CoreConfig.OnAdmin.Invoke(db, info);
        });
        info.UserSetting ??= CoreOption.Instance.UserSetting.Clone();
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
            return Result.Error(CoreLanguage.TipNoUser);

        var attach = new AttachFile(info.File, "Avatars");
        attach.FilePath = $"Avatars/{entity.Id}{attach.ExtName}";
        await attach.SaveAsync();

        var url = Config.GetFileUrl(attach.FilePath);
        entity.SetExtension(nameof(UserInfo.AvatarUrl), url);
        await database.SaveAsync(entity);
        return Result.Success(Language.SaveSuccess, url);
    }

    public async Task<Result> UpdateUserAsync(UserInfo info)
    {
        if (info == null)
            return Result.Error(CoreLanguage.TipNoUser);

        var result = await Database.SaveUserAsync(Context, info);
        if (!result.IsValid)
            return result;

        return Result.Success(Language.SaveSuccess, info);
    }

    public async Task<Result> UpdatePasswordAsync(PwdFormInfo info)
    {
        var user = CurrentUser;
        if (user == null)
            return Result.Error(CoreLanguage.TipNoLogin);

        var errors = new List<string>();
        if (string.IsNullOrEmpty(info.OldPwd))
            errors.Add(Language[CoreLanguage.TipCurPwdRequired]);
        if (string.IsNullOrEmpty(info.NewPwd))
            errors.Add(Language[CoreLanguage.TipNewPwdRequired]);
        if (string.IsNullOrEmpty(info.NewPwd1))
            errors.Add(Language[CoreLanguage.TipConPwdRequired]);
        if (info.NewPwd != info.NewPwd1)
            errors.Add(Language[CoreLanguage.TipPwdNotEqual]);

        if (errors.Count > 0)
            return Result.Error(string.Join(Environment.NewLine, errors));

        var database = Database;
        var entity = await database.QueryByIdAsync<SysUser>(user.Id);
        if (entity == null)
            return Result.Error(CoreLanguage.TipNoUser);

        var oldPwd = Utils.ToMd5(info.OldPwd);
        if (entity.Password != oldPwd)
            return Result.Error(CoreLanguage.TipCurPwdInvalid);

        entity.Password = Utils.ToMd5(info.NewPwd);
        await database.SaveAsync(entity);
        return Result.Success(Language.Success(Language.Update), entity.Id);
    }
}