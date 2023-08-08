namespace Sample.Core.Repositories;

class HomeRepository
{
    internal static int GetUserCount(Database db)
    {
        var sql = "select count(*) from SysUser where CompNo=@CompNo";
        return db.Scalar<int>(sql, new { db.User.CompNo });
    }

    internal static int GetLogCount(Database db)
    {
        var sql = "select count(*) from SysLog where CompNo=@CompNo";
        return db.Scalar<int>(sql, new { db.User.CompNo });
    }

    internal static object GetLogCount(Database db, DateTime date)
    {
        var day = date.ToString("yyyy-MM-dd");
        var sql = $@"select count(1) from SysLog where CompNo=@CompNo and CreateTime between '{day} 00:00:00' and '{day} 23:59:59'";
        if (db.DatabaseType == DatabaseType.Access)
            sql = sql.Replace("'", "#");
        return db.Scalar<int>(sql, new { db.User.CompNo });
    }
}