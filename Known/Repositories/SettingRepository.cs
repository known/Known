namespace Known.Repositories;

class SettingRepository
{
    internal static Task<List<SysSetting>> GetSettingsAsync(Database db, string bizType)
    {
        var sql = "select * from SysSetting where CompNo=@CompNo and BizType=@bizType";
        return db.QueryListAsync<SysSetting>(sql, new { db.User.CompNo, bizType });
    }

    internal static Task<SysSetting> GetSettingByCompAsync(Database db, string bizType)
    {
        var sql = "select * from SysSetting where CompNo=@CompNo and BizType=@bizType";
        return db.QueryAsync<SysSetting>(sql, new { db.User.CompNo, bizType });
    }

    internal static Task<List<SysSetting>> GetSettingsByUserAsync(Database db, string bizType)
    {
        var sql = "select * from SysSetting where CreateBy=@UserName and BizType=@bizType";
        return db.QueryListAsync<SysSetting>(sql, new { db.User.UserName, bizType });
    }

    internal static Task<SysSetting> GetSettingByUserAsync(Database db, string bizType)
    {
        var sql = "select * from SysSetting where CreateBy=@UserName and BizType=@bizType";
        return db.QueryAsync<SysSetting>(sql, new { db.User.UserName, bizType });
    }

    internal static Task<SysSetting> GetSettingByUserAsync(Database db, string bizType, string bizName)
    {
        var sql = "select * from SysSetting where CreateBy=@UserName and BizType=@bizType and BizName=@bizName";
        return db.QueryAsync<SysSetting>(sql, new { db.User.UserName, bizType, bizName });
    }
}