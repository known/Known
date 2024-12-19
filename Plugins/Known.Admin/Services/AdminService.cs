namespace Known.Services;

class AdminService(Context context) : ServiceBase(context), IAdminService
{
    #region Config
    public async Task<bool> GetInstallAsync()
    {
        var db = Database;
        db.EnableLog = false;
        AdminConfig.System ??= await db.GetSystemAsync();
        return AdminConfig.System == null;
    }

    public Task<string> GetConfigAsync(string key)
    {
        return Database.GetConfigAsync(key);
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        return Database.SaveConfigAsync(info.Key, info.Value);
    }
    #endregion

    #region Auth
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
            await db.AddLogAsync(type, $"{user.UserName}-{user.Name}", $"IP：{user.LastLoginIP}");
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
        var info = new AdminInfo
        {
            AppName = await db.GetUserSystemNameAsync(),
            UserMenus = await db.GetUserMenusAsync(),
            UserSetting = await db.GetUserSettingAsync<UserSettingInfo>(Constants.UserSetting),
            UserTableSettings = await db.GetUserTableSettingsAsync(),
            Codes = await db.GetDictionariesAsync()
        };
        await db.CloseAsync();
        Cache.AttachCodes(info.Codes);
        return info;
    }
    #endregion

    #region User
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
    #endregion

    #region Setting
    public async Task<string> GetUserSettingAsync(string bizType)
    {
        var setting = await Database.GetUserSettingAsync(bizType);
        if (setting == null)
            return default;

        return setting.BizData;
    }

    public async Task<Result> SaveUserSettingFormAsync(SettingFormInfo info)
    {
        var database = Database;
        var setting = await database.GetUserSettingAsync(info.BizType);
        setting ??= new SettingInfo();
        setting.BizType = info.BizType;
        setting.BizData = Utils.ToJson(info.BizData);
        await database.SaveSettingAsync(setting);
        return Result.Success(Language.Success(Language.Save));
    }
    #endregion

    #region File
    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Database.GetFilesAsync(bizId);
    }

    public async Task<Result> DeleteFileAsync(AttachInfo file)
    {
        if (file == null || string.IsNullOrWhiteSpace(file.Path))
            return Result.Error(Language["Tip.FileNotExists"]);

        await Database.DeleteFileAsync(file.Id);
        AttachFile.DeleteFile(file.Path);
        return Result.Success(Language.Success(Language.Delete));
    }
    #endregion

    #region Log
    public Task<Result> AddLogAsync(LogInfo log)
    {
        return Database.AddLogAsync(log);
    }
    #endregion
}