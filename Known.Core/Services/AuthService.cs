namespace Known.Core.Services;

class AuthService(Context context) : ServiceBase(context), IAuthService
{
    private static readonly ConcurrentDictionary<string, UserInfo> cachedUsers = new();

    //Account
    public async Task<Result> SignInAsync(LoginFormInfo info)
    {
        var database = Database;
        var userName = info.UserName?.ToLower();
        var entity = await GetUserAsync(database, userName, info.Password);
        if (entity == null)
            return Result.Error(Language["Tip.LoginNoNamePwd"]);

        if (!entity.Enabled)
            return Result.Error(Language["Tip.LoginDisabled"]);

        if (!entity.FirstLoginTime.HasValue)
        {
            entity.FirstLoginTime = DateTime.Now;
            entity.FirstLoginIP = info.IPAddress;
        }
        entity.LastLoginTime = DateTime.Now;
        entity.LastLoginIP = info.IPAddress;

        await database.OpenAsync();
        var user = await entity.ToUserAsync(database);
        user.Token = Utils.GetGuid();
        user.Station = info.Station;
        await database.CloseAsync();
        cachedUsers[user.UserName] = user;

        var type = LogType.Login.ToString();
        if (info.IsMobile)
            type = "APP" + type;

        database.User = user;
        return await database.TransactionAsync(Language["Login"], async db =>
        {
            await db.SaveAsync(entity);
            await AddLogAsync(db, type, $"{user.UserName}-{user.Name}", $"IP：{user.LastLoginIP}");
        }, user);
    }

    public async Task<Result> SignOutAsync(string token)
    {
        var user = CurrentUser;
        if (string.IsNullOrWhiteSpace(token))
            token = user?.Token;

        if (!string.IsNullOrWhiteSpace(token))
            cachedUsers.TryRemove(token, out UserInfo _);

        if (user != null)
        {
            using var db = Database.Create();
            db.User = user;
            await AddLogAsync(db, LogType.Logout.ToString(), $"{user.UserName}-{user.Name}", $"token: {token}");
        }

        return Result.Success(Language["Tip.ExitSuccess"]);
    }

    internal static UserInfo GetUserByToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        return cachedUsers.Values.FirstOrDefault(u => u.Token == token);
    }

    internal static async Task<UserInfo> GetUserAsync(Database db, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return null;

        userName = userName.Split('-')[0];
        cachedUsers.TryGetValue(userName, out UserInfo user);
        if (user == null)
        {
            user = await UserHelper.GetUserByUserNameAsync(db, userName);
            cachedUsers[user.UserName] = user;
        }
        return user;
    }

    public Task<UserInfo> GetUserAsync(string userName) => GetUserAsync(Database, userName);

    public async Task<AdminInfo> GetAdminAsync()
    {
        if (CurrentUser == null)
            return new AdminInfo();

        var database = Database;
        await database.OpenAsync();
        await SystemService.CheckKeyAsync(database);
        var modules = await ModuleService.GetModulesAsync(database);
        DataHelper.Initialize(modules);
        var info = new AdminInfo
        {
            AppName = await UserHelper.GetSystemNameAsync(database),
            MessageCount = await database.CountAsync<SysMessage>(d => d.UserId == database.User.UserName && d.Status == Constant.UMStatusUnread),
            UserMenus = await UserHelper.GetUserMenusAsync(database, modules),
            UserSetting = await SettingService.GetUserSettingAsync<SettingInfo>(database, Constant.UserSetting),
            UserTableSettings = await SettingService.GetUserTableSettingsAsync(database),
            Codes = await DictionaryService.GetDictionariesAsync(database)
        };
        await database.CloseAsync();
        Cache.AttachCodes(info.Codes);
        return info;
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

        entity.Password = Utils.ToMd5(info.NewPwd);
        await database.SaveAsync(entity);
        return Result.Success(Language.Success(Language["Button.Update"]), entity.Id);
    }

    private static Task<SysUser> GetUserAsync(Database db, string userName, string password)
    {
        password = Utils.ToMd5(password);
        return db.QueryAsync<SysUser>(d => d.UserName == userName && d.Password == password);
    }

    private static Task AddLogAsync(Database db, string type, string target, string content)
    {
        return db.SaveAsync(new SysLog
        {
            Type = type,
            Target = target,
            Content = content
        });
    }
}