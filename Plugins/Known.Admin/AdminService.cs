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

    public async Task SaveSettingAsync(Database db, SettingInfo info)
    {
        var model = await db.QueryByIdAsync<SysSetting>(info.Id);
        model ??= new SysSetting();
        if (!string.IsNullOrWhiteSpace(info.Id))
            model.Id = info.Id;
        model.BizType = info.BizType;
        model.BizData = info.BizData;
        await db.SaveAsync(model);
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

    #region Task
    public Task<TaskInfo> GetTaskAsync(Database db, string bizId)
    {
        return db.Query<SysTask>().Where(d => d.CreateBy == db.UserName && d.BizId == bizId)
                 .OrderByDescending(d => d.CreateTime).FirstAsync<TaskInfo>();
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