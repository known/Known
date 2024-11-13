namespace Known.Admin;

class PlatformService : IPlatformService
{
    #region Config
    public async Task<string> GetConfigAsync(Database db, string key)
    {
        var appId = Config.App.Id;
        var config = await db.QueryAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        return config?.ConfigValue;
    }

    public async Task SaveConfigAsync(Database db, string key, object value)
    {
        var appId = Config.App.Id;
        var data = new Dictionary<string, object>();
        data[nameof(SysConfig.AppId)] = appId;
        data[nameof(SysConfig.ConfigKey)] = key;
        data[nameof(SysConfig.ConfigValue)] = Utils.ToJson(value);
        var scalar = await db.CountAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        if (scalar > 0)
            await db.UpdateAsync(nameof(SysConfig), "AppId,ConfigKey", data);
        else
            await db.InsertAsync(nameof(SysConfig), data);
    }
    #endregion

    #region User
    public Task<UserInfo> GetUserAsync(Database db, string userName)
    {
        return db.QueryAsync<UserInfo>(d => d.UserName == userName);
    }
    #endregion

    #region File
    public Task<List<AttachInfo>> GetFilesAsync(Database db, string bizId)
    {
        return FileService.GetFilesAsync(db, bizId);
    }

    public Task<List<AttachInfo>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType)
    {
        return FileService.AddFilesAsync(db, files, bizId, bizType);
    }

    public Task DeleteFileAsync(Database db, string id)
    {
        return db.DeleteAsync<SysFile>(id);
    }

    public Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles)
    {
        return FileService.DeleteFilesAsync(db, bizId, oldFiles);
    }
    #endregion

    #region Task
    public Task<TaskInfo> GetTaskAsync(Database db, string bizId)
    {
        return db.Query<TaskInfo>().Where(d => d.CreateBy == db.UserName && d.BizId == bizId)
                 .OrderByDescending(d => d.CreateTime).FirstAsync();
    }

    public Task CreateTaskAsync(Database db, TaskInfo info)
    {
        return db.CreateTaskAsync(info);
    }
    #endregion

    #region Log
    public async Task<Result> AddLogAsync(Database db, LogInfo log)
    {
        if (log.Type == LogType.Page &&
            string.IsNullOrWhiteSpace(log.Target) &&
            !string.IsNullOrWhiteSpace(log.Content))
        {
            var module = log.Content.StartsWith("/page/")
                       ? await db.QueryByIdAsync<SysModule>(log.Content.Substring(6))
                       : await db.QueryAsync<SysModule>(d => d.Url == log.Content);
            log.Target = module?.Name;
        }

        await db.SaveAsync(new SysLog
        {
            Type = log.Type.ToString(),
            Target = log.Target,
            Content = log.Content
        });
        return Result.Success("");
    }
    #endregion
}