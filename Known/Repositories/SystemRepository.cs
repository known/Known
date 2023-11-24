using Known.Entities;

namespace Known.Repositories;

class SystemRepository
{
    //Config
    internal static Task<string> GetConfigAsync(Database db, string key)
    {
        var appId = Config.App.Id;
        var sql = "select ConfigValue from SysConfig where AppId=@appId and ConfigKey=@key";
        return db.ScalarAsync<string>(sql, new { appId, key });
    }

    internal static async Task<int> SaveConfigAsync(Database db, string key, string value)
    {
        var appId = Config.App.Id;
        var sql = "select count(*) from SysConfig where AppId=@appId and ConfigKey=@key";
        var scalar = await db.ScalarAsync<int>(sql, new { appId, key });
        if (scalar > 0)
        {
            sql = "update SysConfig set ConfigValue=@value where AppId=@appId and ConfigKey=@key";
            return await db.ExecuteAsync(sql, new { value, appId, key });
        }
        else
        {
            sql = "insert into SysConfig(AppId,ConfigKey,ConfigValue) values(@appId,@key,@value)";
            return await db.ExecuteAsync(sql, new { appId, key, value });
        }
    }

    //Task
    internal static Task<PagingResult<SysTask>> QueryTasksAsync(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysTask where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPageAsync<SysTask>(sql, criteria);
    }

    internal static Task<SysTask> GetPendingTaskByTypeAsync(Database db, string type)
    {
        var sql = $"select * from SysTask where Type='{type}' and Status='{TaskStatus.Pending}' order by CreateTime";
        return db.QueryAsync<SysTask>(sql);
    }

    internal static Task<SysTask> GetTaskByTypeAsync(Database db, string type)
    {
        var sql = "select * from SysTask where CompNo=@CompNo and Type=@type order by CreateTime desc";
        return db.QueryAsync<SysTask>(sql, new { db.User.CompNo, type });
    }

    internal static Task<SysTask> GetTaskByBizIdAsync(Database db, string bizId)
    {
        var sql = "select * from SysTask where CompNo=@CompNo and CreateBy=@UserName and BizId=@bizId order by CreateTime desc";
        return db.QueryAsync<SysTask>(sql, new { db.User.CompNo, db.User.UserName, bizId });
    }

    //Log
    internal static Task<PagingResult<SysLog>> QueryLogsAsync(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysLog where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPageAsync<SysLog>(sql, criteria);
    }

    internal static Task<List<CountInfo>> GetLogCountsAsync(Database db, string userName, string logType)
    {
        var sql = "select Target as Field1,count(*) as TotalCount from SysLog where CreateBy=@userName and Type=@logType group by Target";
        return db.QueryListAsync<CountInfo>(sql, new { userName, logType });
    }

    internal static Task<List<SysLog>> GetLogsAsync(Database db, string bizId)
    {
        var sql = "select * from SysLog where Target=@bizId order by CreateTime";
        return db.QueryListAsync<SysLog>(sql, new { bizId });
    }
}