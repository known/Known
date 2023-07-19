namespace Known.Core.Repositories;

class SettingRepository
{
    internal static List<SysSetting> GetSettings(Database db, string bizType)
    {
        var sql = "select * from SysSetting where CompNo=@CompNo and BizType=@bizType";
        return db.QueryList<SysSetting>(sql, new { db.User.CompNo, bizType });
    }

    internal static SysSetting GetSettingByComp(Database db, string bizType)
    {
        var sql = "select * from SysSetting where CompNo=@CompNo and BizType=@bizType";
        return db.Query<SysSetting>(sql, new { db.User.CompNo, bizType });
    }

    internal static List<SysSetting> GetSettingsByUser(Database db, string bizType)
    {
        var sql = "select * from SysSetting where CreateBy=@UserName and BizType=@bizType";
        return db.QueryList<SysSetting>(sql, new { db.User.UserName, bizType });
    }

    internal static SysSetting GetSettingByUser(Database db, string bizType)
    {
        var sql = "select * from SysSetting where CreateBy=@UserName and BizType=@bizType";
        return db.Query<SysSetting>(sql, new { db.User.UserName, bizType });
    }

    internal static SysSetting GetSettingByUser(Database db, string bizType, string bizName)
    {
        var sql = "select * from SysSetting where CreateBy=@UserName and BizType=@bizType and BizName=@bizName";
        return db.Query<SysSetting>(sql, new { db.User.UserName, bizType, bizName });
    }
}