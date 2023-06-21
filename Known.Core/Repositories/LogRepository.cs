namespace Known.Core.Repositories;

class LogRepository
{
    internal static PagingResult<SysLog> QueryLogs(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysLog where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPage<SysLog>(sql, criteria);
    }

    internal static List<CountInfo> GetLogCounts(Database db, string userName, string logType)
    {
        var sql = "select Target as Field1,count(*) as TotalCount from SysLog where CreateBy=@userName and Type=@logType group by Target";
        return db.QueryList<CountInfo>(sql, new { userName, logType });
    }

    internal static List<SysLog> GetLogs(Database db, string bizId)
    {
        var sql = "select * from SysLog where Target=@bizId order by CreateTime";
        return db.QueryList<SysLog>(sql, new { bizId });
    }
}