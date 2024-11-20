namespace Known.Platforms;

class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
    #region Config
    public Task<string> GetConfigAsync(string key)
    {
        return Database.GetConfigAsync(key);
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        return Database.SaveConfigAsync(info.Key, info.Value);
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