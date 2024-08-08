namespace Sample.Web.Repositories;

class HomeRepository
{
    internal static Task<int> GetUserCountAsync(Database db)
    {
        return db.ScalarAsync<int>("select count(*) from SysUser where CompNo=@CompNo", new { db.User.CompNo });
    }

    internal static Task<int> GetLogCountAsync(Database db)
    {
        return db.ScalarAsync<int>("select count(*) from SysLog where CompNo=@CompNo", new { db.User.CompNo });
    }

    internal static Task<int> GetLogCountAsync(Database db, DateTime date)
    {
        var day = date.ToString("yyyy-MM-dd");
        var sql = $@"select count(*) from SysLog where CompNo=@CompNo and CreateTime between '{day} 00:00:00' and '{day} 23:59:59'";
        if (db.DatabaseType == DatabaseType.Access)
            sql = sql.Replace("'", "#");
        return db.ScalarAsync<int>(sql, new { db.User.CompNo });
    }
}