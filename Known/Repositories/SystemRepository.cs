namespace Known.Repositories;

class SystemRepository
{
    //Config
    internal static async Task<string> GetConfigAsync(Database db, string key)
    {
        var appId = Config.App.Id;
        var config = await db.QueryAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        return config?.ConfigValue;
    }

    internal static async Task<int> SaveConfigAsync(Database db, string key, string value)
    {
        var appId = Config.App.Id;
        var data = new Dictionary<string, object>();
        data["AppId"] = appId;
        data["ConfigKey"] = key;
        data["ConfigValue"] = value;
        var scalar = await db.CountAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        if (scalar > 0)
            return await db.UpdateAsync("SysConfig", "AppId,ConfigKey", data);
        else
            return await db.InsertAsync("SysConfig", data);
    }

    //Task
    internal static Task<SysTask> GetPendingTaskByTypeAsync(Database db, string type)
    {
        return db.Query<SysTask>()
                 .Where(d => d.Type == type && d.Status == TaskStatus.Pending)
                 .OrderBy(d => d.CreateTime)
                 .FirstAsync();
    }

    internal static Task<SysTask> GetTaskByTypeAsync(Database db, string type)
    {
        return db.Query<SysTask>()
                 .Where(d => d.CompNo == db.User.CompNo && d.Type == type)
                 .OrderByDescending(d => d.CreateTime)
                 .FirstAsync();
    }

    internal static Task<SysTask> GetTaskByBizIdAsync(Database db, string bizId)
    {
        return db.Query<SysTask>()
                 .Where(d => d.CompNo == db.User.CompNo && d.CreateBy == db.UserName && d.BizId == bizId)
                 .OrderByDescending(d => d.CreateTime)
                 .FirstAsync();
    }

    //Log
    internal static Task<List<CountInfo>> GetLogCountsAsync(Database db, string userName, string logType)
    {
        var sql = "select Target as Field1,count(*) as TotalCount from SysLog where CreateBy=@userName and Type=@logType group by Target";
        return db.QueryListAsync<CountInfo>(sql, new { userName, logType });
    }
}