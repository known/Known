namespace Known.Services;

public interface IAuthService : IService
{
    [AllowAnonymous] Task<Result> SignInAsync(LoginFormInfo info);
    Task<Result> SignOutAsync(string token);
    Task<UserInfo> GetUserAsync(string userName);
    Task<AdminInfo> GetAdminAsync();
    Task<Result> UpdatePasswordAsync(PwdFormInfo info);
}

class AuthService(Context context) : ServiceBase(context), IAuthService
{
    //Account
    public async Task<Result> SignInAsync(LoginFormInfo info)
    {
        var userName = info.UserName.ToLower();
        var entity = await GetUserAsync(Database, userName, info.Password);
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

        var user = Utils.MapTo<UserInfo>(entity);
        user.Token = Utils.GetGuid();
        user.Station = info.Station;
        var database = Database;
        await database.OpenAsync();
        await SetUserInfoAsync(database, user);
        await SetUserWeixinAsync(database, user);
        await database.CloseAsync();
        cachedUsers[user.Token] = user;

        var type = LogType.Login.ToString();
        if (info.IsMobile)
            type = "APP" + type;

        database.User = user;
        return await database.TransactionAsync(Language["Login"], async db =>
        {
            await db.SaveAsync(entity);
            await Logger.AddLogAsync(db, type, $"{user.UserName}-{user.Name}", $"IP：{user.LastLoginIP}");
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
            await Logger.AddLogAsync(Database, LogType.Logout.ToString(), $"{user.UserName}-{user.Name}", $"token: {token}");
        return Result.Success(Language["Tip.ExitSuccess"]);
    }

    private static readonly ConcurrentDictionary<string, UserInfo> cachedUsers = new();

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
        var user = cachedUsers.Values.FirstOrDefault(u => u.UserName == userName);
        if (user != null)
            return user;

        user = await db.QueryAsync<UserInfo>(d => d.UserName == userName);
        await SetUserInfoAsync(db, user);
        return user;
    }

    public Task<UserInfo> GetUserAsync(string userName) => GetUserAsync(Database, userName);

    public async Task<AdminInfo> GetAdminAsync()
    {
        if (CurrentUser == null)
            return new AdminInfo();

        var db = Database;
        await db.OpenAsync();
        var modules = await ModuleService.GetModulesAsync(db);
        DataHelper.Initialize(modules);
        var info = new AdminInfo
        {
            AppName = await UserHelper.GetSystemNameAsync(db),
            MessageCount = await db.CountAsync<SysMessage>(d => d.UserId == db.UserName && d.Status == Constants.UMStatusUnread),
            UserMenus = await UserHelper.GetUserMenusAsync(db, modules),
            UserSetting = await SettingService.GetUserSettingAsync<SettingInfo>(db, SettingInfo.KeyInfo),
            Codes = await DictionaryService.GetDictionariesAsync(db)
        };
        await db.CloseAsync();
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
            return Result.Error(string.Join(Environment.NewLine, errors.ToArray()));

        var entity = await GetUserAsync(Database, user.UserName, info.OldPwd);
        if (entity == null)
            return Result.Error(Language["Tip.CurPwdInvalid"]);

        entity.Password = Utils.ToMd5(info.NewPwd);
        await Database.SaveAsync(entity);
        return Result.Success(Language.Success(Language["Button.Update"]), entity.Id);
    }

    private static async Task SetUserInfoAsync(Database db, UserInfo user)
    {
        var info = await SystemService.GetSystemAsync(db);
        user.AvatarUrl = user.Gender == "Female" ? "img/face2.png" : "img/face1.png";
        user.IsTenant = user.CompNo != info.CompNo;
        user.AppName = info.AppName;
        if (user.IsAdmin)
            user.AppId = Config.App.Id;
        user.CompName = info.CompName;
        if (!string.IsNullOrEmpty(user.OrgNo))
        {
            var org = await db.QueryAsync<SysOrganization>(d => d.AppId == user.AppId && d.CompNo == user.CompNo && d.Code == user.OrgNo);
            var orgName = org?.Name ?? user.CompName;
            user.OrgName = orgName;
            if (string.IsNullOrEmpty(user.CompName))
                user.CompName = orgName;
        }
    }

    private static async Task SetUserWeixinAsync(Database db, UserInfo user)
    {
        var weixin = await WeixinService.GetWeixinByUserIdAsync(db, user.Id);
        if (weixin == null)
            return;

        user.OpenId = weixin.OpenId;
        user.AvatarUrl = weixin.HeadImgUrl;
    }

    private static Task<SysUser> GetUserAsync(Database db, string userName, string password)
    {
        password = Utils.ToMd5(password);
        return db.QueryAsync<SysUser>(d => d.UserName == userName && d.Password == password);
    }
}