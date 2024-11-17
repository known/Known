namespace Known;

class AdminService : IAdminService
{
    #region Config
    public Task<string> GetConfigAsync(Database db, string key)
    {
        return db.GetConfigAsync(key);
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

    #region Module
    public Task<List<ModuleInfo>> GetModulesAsync(Database db)
    {
        return db.Query<SysModule>().ToListAsync<ModuleInfo>();
    }
    #endregion

    #region Company
    public async Task<string> GetCompanyDataAsync(Database db, string compNo)
    {
        var model = await db.QueryAsync<SysCompany>(d => d.Code == compNo);
        if (model == null)
            return string.Empty;

        var data = model.CompanyData;
        if (string.IsNullOrWhiteSpace(data))
        {
            data = Utils.ToJson(new
            {
                model.Code,
                model.Name,
                model.NameEn,
                model.SccNo,
                model.Address,
                model.AddressEn
            });
        }
        return data;
    }
    #endregion

    #region Role
    public Task<List<string>> GetRoleModuleIdsAsync(Database db, string userId)
    {
        var sql = $@"select a.{db.FormatName("ModuleId")} from {db.FormatName("SysRoleModule")} a 
where a.{db.FormatName("RoleId")} in (select {db.FormatName("RoleId")} from {db.FormatName("SysUserRole")} where {db.FormatName("UserId")}=@UserId)
  and exists (select 1 from {db.FormatName("SysRole")} where {db.FormatName("Id")}=a.{db.FormatName("RoleId")} and {db.FormatName("Enabled")}='True')";
        return db.ScalarsAsync<string>(sql, new { UserId = userId });
    }
    #endregion

    #region User
    public async Task<UserInfo> GetUserAsync(Database db, string userName)
    {
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName);
        return await GetUserInfoAsync(db, user);
    }

    public async Task<UserInfo> GetUserByIdAsync(Database db, string userId)
    {
        var user = await db.QueryAsync<SysUser>(d => d.Id == userId);
        return await GetUserInfoAsync(db, user);
    }

    public async Task<UserInfo> GetUserAsync(Database db, string userName, string password)
    {
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName && d.Password == password);
        return await GetUserInfoAsync(db, user);
    }

    private async Task<UserInfo> GetUserInfoAsync(Database db, SysUser user)
    {
        if (user == null)
            return null;

        var info = Utils.MapTo<UserInfo>(user);
        var avatarUrl = user.GetExtension<string>(nameof(UserInfo.AvatarUrl));
        if (string.IsNullOrWhiteSpace(avatarUrl))
            avatarUrl = user.Gender == "Female" ? "img/face2.png" : "img/face1.png";
        info.AvatarUrl = avatarUrl;
        await SetUserInfoAsync(db, info);
        //await user.SetUserWeixinAsync(db);
        return info;
    }

    private async Task SetUserInfoAsync(Database db, UserInfo user)
    {
        var info = await this.GetSystemAsync(db);
        user.IsTenant = user.CompNo != info?.CompNo;
        user.AppName = info?.AppName;
        if (user.IsAdmin())
            user.AppId = Config.App.Id;
        user.CompName = info?.CompName;
        if (!string.IsNullOrEmpty(user.OrgNo))
        {
            var org = await db.QueryAsync<SysOrganization>(d => d.CompNo == user.CompNo && d.Code == user.OrgNo);
            var orgName = org?.Name ?? user.CompName;
            user.OrgName = orgName;
            if (string.IsNullOrEmpty(user.CompName))
                user.CompName = orgName;
        }
    }

    public async Task<Result> SaveUserAsync(Database db, UserInfo info)
    {
        var model = await db.QueryByIdAsync<SysUser>(info.Id);
        if (model == null)
            return Result.Error(db.Context.Language["Tip.NoUser"]);

        model.Name = info.Name;
        model.EnglishName = info.EnglishName;
        model.Gender = info.Gender;
        model.Phone = info.Phone;
        model.Mobile = info.Mobile;
        model.Email = info.Email;
        model.Note = info.Note;
        if (!info.FirstLoginTime.HasValue)
        {
            model.FirstLoginTime = info.FirstLoginTime;
            model.FirstLoginIP = info.FirstLoginIP;
        }
        model.LastLoginTime = info.LastLoginTime;
        model.LastLoginIP = info.LastLoginIP;

        var vr = model.Validate(db.Context);
        if (!vr.IsValid)
            return vr;

        await db.SaveAsync(model);
        return Result.Success("保存成功！");
    }

    public async Task<Result> SaveUserAvatarAsync(Database db, string userId, string url)
    {
        var model = await db.QueryByIdAsync<SysUser>(userId);
        if (model == null)
            return Result.Error(db.Context.Language["Tip.NoUser"]);

        model.SetExtension(nameof(UserInfo.AvatarUrl), url);
        await db.SaveAsync(model);
        return Result.Success("保存成功！");
    }

    public async Task<Result> SaveUserPasswordAsync(Database db, string userId, string password)
    {
        var model = await db.QueryByIdAsync<SysUser>(userId);
        if (model == null)
            return Result.Error(db.Context.Language["Tip.NoUser"]);

        model.Password = password;
        await db.SaveAsync(model);
        return Result.Success("保存成功！");
    }
    #endregion

    #region Setting
    public Task<List<SettingInfo>> GetUserSettingsAsync(Database db, string bizTypePrefix)
    {
        return db.Query<SysSetting>()
                 .Where(d => d.CreateBy == db.UserName && d.BizType.StartsWith(bizTypePrefix))
                 .ToListAsync<SettingInfo>();
    }

    public Task<SettingInfo> GetUserSettingAsync(Database db, string bizType)
    {
        return db.Query<SysSetting>()
                 .Where(d => d.CreateBy == db.UserName && d.BizType == bizType)
                 .FirstAsync<SettingInfo>();
    }

    public Task DeleteSettingAsync(Database db, string id)
    {
        return db.DeleteAsync<SysSetting>(id);
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