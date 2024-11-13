namespace Known.Core.Services;

class AuthService(Context context) : ServiceBase(context), IAuthService
{
    //Account
    public async Task<Result> SignInAsync(LoginFormInfo info)
    {
        var database = Database;
        var userName = info.UserName?.ToLower();
        var user = await GetUserAsync(database, userName, info.Password);
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

        await database.OpenAsync();
        user.Token = Utils.GetGuid();
        user.Station = info.Station;
        await database.CloseAsync();
        Cache.SetUser(user);

        var type = LogType.Login;
        if (info.IsMobile)
            type = LogType.AppLogin;

        database.User = user;
        return await database.TransactionAsync(Language["Login"], async db =>
        {
            await Platform.SaveUserAsync(db, user);
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

    internal static async Task<UserInfo> GetUserAsync(IPlatformService platform, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return null;

        userName = userName.Split('-')[0];
        var user = Cache.GetUser(userName);
        if (user == null)
        {
            var db = Database.Create();
            user = await platform.GetUserAsync(db, userName);
            Cache.SetUser(user);
        }
        return user;
    }

    public async Task<AdminInfo> GetAdminAsync()
    {
        if (CurrentUser == null)
            return new AdminInfo();

        var database = Database;
        await database.OpenAsync();
        await Platform.CheckKeyAsync(database);
        var modules = await ModuleService.GetModulesAsync(database);
        DataHelper.Initialize(modules);
        var info = new AdminInfo
        {
            AppName = await Platform.GetSystemNameAsync(database),
            UserMenus = await UserHelper.GetUserMenusAsync(database, modules),
            UserSetting = await Platform.GetUserSettingAsync<UserSettingInfo>(database, Constant.UserSetting),
            UserTableSettings = await Platform.GetUserTableSettingsAsync(database)
        };
        await SetAdminAsync(database, info);
        await database.CloseAsync();
        Cache.AttachCodes(info.Codes);
        return info;
    }

    public async Task<Result> UpdateAvatarAsync(AvatarInfo info)
    {
        var database = Database;
        var entity = await Platform.GetUserByIdAsync(database, info.UserId);
        if (entity == null)
            return Result.Error(Language["Tip.NoUser"]);

        var attach = new AttachFile(info.File, CurrentUser);
        attach.FilePath = @$"Avatars\{entity.Id}{attach.ExtName}";
        await attach.SaveAsync();

        var url = Config.GetFileUrl(attach.FilePath);
        var result = await Platform.SaveUserAvatarAsync(database, info.UserId, url);
        if (!result.IsValid)
            return result;

        return Result.Success(Language.Success(Language.Save), url);
    }

    public async Task<Result> UpdateUserAsync(UserInfo info)
    {
        if (info == null)
            return Result.Error(Language["Tip.NoUser"]);

        var result = await Platform.SaveUserAsync(Database, info);
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
        var entity = await GetUserAsync(database, user.UserName, info.OldPwd);
        if (entity == null)
            return Result.Error(Language["Tip.CurPwdInvalid"]);

        var password = Utils.ToMd5(info.NewPwd);
        var result = await Platform.SaveUserPasswordAsync(database, entity.Id, password);
        if (!result.IsValid)
            return result;

        return Result.Success(Language.Success(Language["Button.Update"]), entity.Id);
    }

    private Task<UserInfo> GetUserAsync(Database db, string userName, string password)
    {
        password = Utils.ToMd5(password);
        return Platform.GetUserAsync(db, userName, password);
    }

    private static async Task SetAdminAsync(Database db, AdminInfo info)
    {
        if (Config.AdminTasks == null || Config.AdminTasks.Count == 0)
            return;

        foreach (var item in Config.AdminTasks)
        {
            await item.Value.Invoke(db, info);
        }
    }

    private Task AddLogAsync(Database db, LogType type, string target, string content)
    {
        return Platform.AddLogAsync(db, new LogInfo
        {
            Type = type,
            Target = target,
            Content = content
        });
    }
}