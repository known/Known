namespace Known;

class AdminService : IAdminService
{
    #region Config
    public Task<string> GetConfigAsync(Database db, string key)
    {
        return db.GetConfigAsync(key);
    }

    public Task SaveConfigAsync(Database db, string key, object value)
    {
        return db.SaveConfigAsync(key, value);
    }
    #endregion

    #region User
    public async Task<UserInfo> GetUserAsync(Database db, string userName)
    {
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName);
        return await db.GetUserInfoAsync(user);
    }

    public async Task<UserInfo> GetUserByIdAsync(Database db, string userId)
    {
        var user = await db.QueryAsync<SysUser>(d => d.Id == userId);
        return await db.GetUserInfoAsync(user);
    }
    #endregion

    #region Setting
    public Task<SettingInfo> GetUserSettingAsync(Database db, string bizType)
    {
        return db.GetUserSettingAsync(bizType);
    }

    public Task SaveSettingAsync(Database db, SettingInfo info)
    {
        return db.SaveSettingAsync(info);
    }
    #endregion

    #region File
    public Task<List<AttachInfo>> GetFilesAsync(Database db, string bizId)
    {
        return FileService.GetFilesAsync(db, bizId);
    }

    public Task<List<AttachInfo>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType)
    {
        return db.AddFilesAsync(files, bizId, bizType);
    }

    public Task DeleteFileAsync(Database db, string id)
    {
        return db.DeleteAsync<SysFile>(id);
    }

    public Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles)
    {
        return db.DeleteFilesAsync(bizId, oldFiles);
    }
    #endregion

    #region Log
    public Task<Result> AddLogAsync(Database db, LogInfo log)
    {
        return db.AddLogAsync(log);
    }
    #endregion
}